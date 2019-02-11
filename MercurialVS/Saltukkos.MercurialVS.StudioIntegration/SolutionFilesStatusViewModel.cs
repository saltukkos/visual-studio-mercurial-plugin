using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.MercurialVS.SourceControl;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    public sealed class SolutionFilesStatusViewModel : IDisposable
    {
        [NotNull]
        private readonly IDirectoryStateProvider _directoryStateProvider;

        [NotNull]
        private readonly IVsDifferenceService _vsDifferenceService;

        [NotNull]
        private readonly IFileHistoryProvider _fileHistoryProvider;

        [NotNull]
        private readonly IVsUIShellOpenDocument _uiShellOpenDocument;

        [NotNull]
        private readonly Dispatcher _dispatcher;

        [NotNull]
        public ObservableCollection<FileState> Files { get; } = new ObservableCollection<FileState>();

        public FileState SelectedItem { get; set; }

        public SolutionFilesStatusViewModel(
            [NotNull] IDirectoryStateProvider directoryStateProvider,
            [NotNull] IVsDifferenceService vsDifferenceService,
            [NotNull] IFileHistoryProvider fileHistoryProvider,
            [NotNull] IVsUIShellOpenDocument uiShellOpenDocument)
        {
            ThrowIf.Null(directoryStateProvider, nameof(directoryStateProvider));
            ThrowIf.Null(vsDifferenceService, nameof(vsDifferenceService));
            ThrowIf.Null(fileHistoryProvider, nameof(fileHistoryProvider));
            ThrowIf.Null(uiShellOpenDocument, nameof(uiShellOpenDocument));
            _directoryStateProvider = directoryStateProvider;
            _vsDifferenceService = vsDifferenceService;
            _fileHistoryProvider = fileHistoryProvider;
            _uiShellOpenDocument = uiShellOpenDocument;

            _dispatcher = Dispatcher.CurrentDispatcher;
            _directoryStateProvider.DirectoryStateChanged += SetFiles;
        }

        private void SetFiles(object sender, EventArgs e)
        {
            var fileStates = _directoryStateProvider
                .CurrentStatus
                .Where(f => f.Status != FileStatus.Clean && f.Status != FileStatus.Ignored);

            _dispatcher.Invoke(() =>
            {
                Files.Clear();
                foreach (var item in fileStates)
                {
                    Files.Add(item);
                }
            }, DispatcherPriority.Normal);
        }

        public void Dispose()
        {
            _directoryStateProvider.DirectoryStateChanged -= SetFiles;
        }

        public void OnItemClicked()
        {
            var selectedItemFilePath = SelectedItem.FilePath;
            var name = Path.GetFileName(selectedItemFilePath);
            switch (SelectedItem.Status)
            {
                case FileStatus.Unknown:
                case FileStatus.Added:
                case FileStatus.Clean:
                case FileStatus.Ignored:
                    OpenFileInEditor(selectedItemFilePath);
                    break;
                case FileStatus.Modified:
                    _fileHistoryProvider.ExecuteWithFileAtCurrentRevision(selectedItemFilePath, oldFile =>
                    {
                        using (TemporaryFilesScopeCookie())
                        {
                            _vsDifferenceService.OpenComparisonWindow2(
                                leftFileMoniker: oldFile,
                                rightFileMoniker: selectedItemFilePath,
                                caption: $"Diff - {name}",
                                Tooltip: $"{name}: current revision - changed",
                                leftLabel: $"{name}: at current revision",
                                rightLabel: $"{name}: changed version",
                                inlineLabel: $"{name}: current revision - changed",
                                roles: null,
                                grfDiffOptions: (uint)(__VSDIFFSERVICEOPTIONS.VSDIFFOPT_LeftFileIsTemporary));
                        }
                    });
                    break;
                case FileStatus.Removed:
                case FileStatus.Missing:
                    _fileHistoryProvider.ExecuteWithFileAtCurrentRevision(selectedItemFilePath, oldFile =>
                    {
                        using (TemporaryFilesScopeCookie())
                        {
                            OpenFileInEditor(oldFile);
                        }
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(SelectedItem.Status), SelectedItem.Status, null);
            }
        }

        private void OpenFileInEditor([NotNull] string selectedItemFilePath)
        {
            var __ = Guid.Empty;
            _uiShellOpenDocument
                .OpenDocumentViaProject(selectedItemFilePath, ref __, out _, out _, out _, out var frame);
            //TODO readonly and readable name
            //var vsTextView = VsShellUtilities.GetTextView(frame);
            frame?.Show();
        }

        [NotNull]
        private static NewDocumentStateScope TemporaryFilesScopeCookie()
        {
            return new NewDocumentStateScope(__VSNEWDOCUMENTSTATE.NDS_Provisional, VSConstants.NewDocumentStateReason.TeamExplorer);
        }
    }
}
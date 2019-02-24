using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.MercurialVS.SourceControl;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.SolutionFilesStatus
{
    public sealed class SolutionFilesStatusViewModel : IDisposable, INotifyPropertyChanged
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
        private readonly IVsIdleNotifier _idleNotifier;

        [NotNull]
        private readonly object _outdatedFilesSyncRoot = new object();

        private bool _filesListOutdated = true;

        private FileStateView _selectedItem;

        public SolutionFilesStatusViewModel(
            [NotNull] IDirectoryStateProvider directoryStateProvider,
            [NotNull] IVsDifferenceService vsDifferenceService,
            [NotNull] IFileHistoryProvider fileHistoryProvider,
            [NotNull] IVsUIShellOpenDocument uiShellOpenDocument,
            [NotNull] IVsIdleNotifier idleNotifier)
        {
            ThrowIf.Null(directoryStateProvider, nameof(directoryStateProvider));
            ThrowIf.Null(vsDifferenceService, nameof(vsDifferenceService));
            ThrowIf.Null(fileHistoryProvider, nameof(fileHistoryProvider));
            ThrowIf.Null(uiShellOpenDocument, nameof(uiShellOpenDocument));
            ThrowIf.Null(idleNotifier, nameof(idleNotifier));

            _directoryStateProvider = directoryStateProvider;
            _vsDifferenceService = vsDifferenceService;
            _fileHistoryProvider = fileHistoryProvider;
            _uiShellOpenDocument = uiShellOpenDocument;
            _idleNotifier = idleNotifier;

            _idleNotifier.IdlingStarted += UpdateFilesList;
            _directoryStateProvider.DirectoryStateChanged += SetFiles;
        }

        [NotNull] public ObservableCollection<FileStateView> Files { get; } = new ObservableCollection<FileStateView>();

        public FileStateView SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanDiff));
            }
        }

        public bool CanDiff => SelectedItem.Status == FileStatus.Modified;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            _directoryStateProvider.DirectoryStateChanged -= SetFiles;
            _idleNotifier.IdlingStarted -= UpdateFilesList;
        }

        public void OnOpenClicked()
        {
            HandleFileClick(diffRequested: false);
        }

        public void OnDiffClicked()
        {
            if (!CanDiff)
            {
                return;
            }

            HandleFileClick(diffRequested: true);
        }

        public void OnItemClicked()
        {
            HandleFileClick(diffRequested: true);
        }

        private void UpdateFilesList()
        {
            lock (_outdatedFilesSyncRoot)
            {
                if (!_filesListOutdated)
                {
                    return;
                }

                var fileStateViews = _directoryStateProvider
                    .CurrentStatus
                    .Where(f => f.Status != FileStatus.Clean && f.Status != FileStatus.Ignored)
                    .Select(f => new FileStateView
                    {
                        Status = f.Status,
                        FileName = Path.GetFileName(f.FilePath),
                        RelativePath = f.RelativePath,
                        FullPath = f.FilePath
                    });

                Files.Clear();
                foreach (var fileStateView in fileStateViews)
                {
                    Files.Add(fileStateView);
                }

                _filesListOutdated = false;
            }
        }

        private void HandleFileClick(bool diffRequested)
        {
            var selectedItemFilePath = SelectedItem.FullPath;
            var name = SelectedItem.FileName;
            switch (SelectedItem.Status)
            {
                case FileStatus.Unknown:
                case FileStatus.Added:
                case FileStatus.Clean:
                case FileStatus.Ignored:
                    OpenFileInEditor(selectedItemFilePath);
                    break;
                case FileStatus.Modified when diffRequested:
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
                                grfDiffOptions: (uint) (__VSDIFFSERVICEOPTIONS.VSDIFFOPT_LeftFileIsTemporary));
                        }
                    });
                    break;
                case FileStatus.Modified:
                    OpenFileInEditor(selectedItemFilePath);
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

        private void SetFiles(object sender, EventArgs e)
        {
            lock (_outdatedFilesSyncRoot)
            {
                _filesListOutdated = true;
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
            return new NewDocumentStateScope(__VSNEWDOCUMENTSTATE.NDS_Provisional,
                VSConstants.NewDocumentStateReason.TeamExplorer);
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
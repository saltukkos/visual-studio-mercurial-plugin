using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using JetBrains.Annotations;
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
        private readonly Dispatcher _dispatcher;

        [NotNull]
        public ObservableCollection<FileState> Files { get; } = new ObservableCollection<FileState>();

        public FileState SelectedItem { get; set; }

        public SolutionFilesStatusViewModel(
            [NotNull] IDirectoryStateProvider directoryStateProvider,
            [NotNull] IVsDifferenceService vsDifferenceService,
            [NotNull] IFileHistoryProvider fileHistoryProvider)
        {
            ThrowIf.Null(directoryStateProvider, nameof(directoryStateProvider));
            ThrowIf.Null(vsDifferenceService, nameof(vsDifferenceService));
            ThrowIf.Null(fileHistoryProvider, nameof(fileHistoryProvider));
            _directoryStateProvider = directoryStateProvider;
            _vsDifferenceService = vsDifferenceService;
            _fileHistoryProvider = fileHistoryProvider;

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
            
            _fileHistoryProvider.ExecuteWithFileAtCurrentRevision(selectedItemFilePath, oldFile =>
            {
                _vsDifferenceService
                    .OpenComparisonWindowFromCommandLineArguments(
                        $"\"{selectedItemFilePath}\" " +
                        $@"""{oldFile}"" label1 label2 inlinelabel");
            });
        }
    }
}
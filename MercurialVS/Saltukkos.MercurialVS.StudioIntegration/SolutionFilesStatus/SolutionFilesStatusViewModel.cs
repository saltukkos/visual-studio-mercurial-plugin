using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using JetBrains.Annotations;
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
        private readonly IOpenFileService _openFileService;

        [NotNull]
        private readonly IVsIdleNotifier _idleNotifier;

        [NotNull]
        private readonly ObservableCollection<FileStateView> _files = new ObservableCollection<FileStateView>();

        [NotNull]
        private readonly CollectionViewSource _filesViewSource = new CollectionViewSource();

        [NotNull]
        private readonly object _outdatedFilesSyncRoot = new object();

        [NotNull]
        private string _filter = string.Empty;

        private bool _filesListOutdated = true;

        private FileStateView _selectedItem;

        public SolutionFilesStatusViewModel(
            [NotNull] IDirectoryStateProvider directoryStateProvider,
            [NotNull] IOpenFileService openFileService,
            [NotNull] IVsIdleNotifier idleNotifier)
        {
            ThrowIf.Null(openFileService, nameof(openFileService));
            ThrowIf.Null(directoryStateProvider, nameof(directoryStateProvider));
            ThrowIf.Null(idleNotifier, nameof(idleNotifier));

            _directoryStateProvider = directoryStateProvider;
            _openFileService = openFileService;
            _idleNotifier = idleNotifier;

            _filesViewSource.Source = _files;
            _filesViewSource.Filter += FilesViewSourceOnFilter;

            _idleNotifier.IdlingStarted += UpdateFilesList;
            _directoryStateProvider.DirectoryStateChanged += SetFiles;
        }

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

        public ICollectionView Files => _filesViewSource.View;

        public bool CanDiff => SelectedItem.Status == FileStatus.Modified;

        [NotNull]
        public string Filter
        {
            get => _filter;
            set
            {
                ThrowIf.Null(value, nameof(value));
                _filter = value;
                OnPropertyChanged();
                _filesViewSource.View.Refresh();
            }
        }

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

                _files.Clear();
                foreach (var fileStateView in fileStateViews)
                {
                    _files.Add(fileStateView);
                }

                _filesListOutdated = false;
            }
        }

        private void HandleFileClick(bool diffRequested)
        {
            var selectedItemFilePath = SelectedItem.FullPath;
            switch (SelectedItem.Status)
            {
                case FileStatus.Unknown:
                case FileStatus.Added:
                case FileStatus.Clean:
                case FileStatus.Ignored:
                    _openFileService.OpenFileFromWorkingDirectory(selectedItemFilePath);
                    break;
                case FileStatus.Modified when diffRequested:
                    _openFileService.OpenFileDiff(selectedItemFilePath, Revision.Current);
                    break;
                case FileStatus.Modified:
                    _openFileService.OpenFileFromWorkingDirectory(selectedItemFilePath);
                    break;
                case FileStatus.Removed:
                case FileStatus.Missing:
                    _openFileService.OpenFileFromRevision(selectedItemFilePath, Revision.Current);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(SelectedItem.Status), SelectedItem.Status, null);
            }
        }

        private void FilesViewSourceOnFilter(object sender, [NotNull] FilterEventArgs e)
        {
            ThrowIf.Null(e, nameof(e));

            if (string.IsNullOrEmpty(_filter))
            {
                e.Accepted = true;
                return;
            }

            if (!(e.Item is FileStateView stateView))
            {
                e.Accepted = true;
                return;
            }

            e.Accepted = stateView.FullPath.Contains(_filter, StringComparison.OrdinalIgnoreCase);
        }

        private void SetFiles(object sender, EventArgs e)
        {
            lock (_outdatedFilesSyncRoot)
            {
                _filesListOutdated = true;
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
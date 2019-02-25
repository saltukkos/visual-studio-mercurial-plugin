﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly object _outdatedFilesSyncRoot = new object();

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
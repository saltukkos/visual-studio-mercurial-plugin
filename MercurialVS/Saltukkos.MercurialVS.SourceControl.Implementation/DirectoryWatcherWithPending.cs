using System;
using System.IO;
using System.Threading;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    public sealed class DirectoryWatcherWithPending : IDirectoryWatcherWithPending
    {
        private readonly int _pendingInMilliseconds;

        private bool _isPending;

        [NotNull]
        private readonly object _notifyingSyncRoot = new object();

        [NotNull]
        private readonly Timer _pendingTimer;

        [CanBeNull]
        private readonly Predicate<string> _includeFilter;

        [NotNull]
        private readonly FileSystemWatcher _fileSystemWatcher;

        public string Path
        {
            get => _fileSystemWatcher.Path ?? string.Empty;
            set => _fileSystemWatcher.Path = value;
        }

        public bool RaiseEvents
        {
            get
            {
                return _fileSystemWatcher.EnableRaisingEvents;
            }
            set
            {
                _fileSystemWatcher.EnableRaisingEvents = value;
                if (value == false)
                {
                    StopPendingTimer();
                }
            }
        }

        public event EventHandler OnDirectoryChanged;

        public DirectoryWatcherWithPending(
            int pendingInMilliseconds,
            [CanBeNull] Predicate<string> includeFilter = null)
        {
            _pendingInMilliseconds = pendingInMilliseconds;
            _pendingTimer = new Timer(OnPendingFinished, null, Timeout.Infinite, Timeout.Infinite);
            _includeFilter = includeFilter;
            _fileSystemWatcher = new FileSystemWatcher
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName
            };
            _fileSystemWatcher.Changed += OnChanged;
            _fileSystemWatcher.Created += OnChanged;
            _fileSystemWatcher.Deleted += OnChanged;
            _fileSystemWatcher.Renamed += OnChanged;
        }

        private void OnPendingFinished(object state)
        {
            lock (_notifyingSyncRoot)
            {
                StopPendingTimer();
                OnDirectoryChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (_isPending)
            {
                return;
            }

            if (_includeFilter?.Invoke(e.Name) == false)
            {
                return;
            }

            lock (_notifyingSyncRoot)
            {
                StartPendingTimer();
            }
        }

        private void StartPendingTimer()
        {
            _isPending = true;
            _pendingTimer.Change(_pendingInMilliseconds, Timeout.Infinite);
        }

        private void StopPendingTimer()
        {
            _isPending = false;
            _pendingTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            _fileSystemWatcher.Dispose();
            _pendingTimer.Dispose();
        }
    }
}
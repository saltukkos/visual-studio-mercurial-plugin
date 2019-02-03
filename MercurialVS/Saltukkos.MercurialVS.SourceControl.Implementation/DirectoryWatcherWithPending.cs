using System;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [Component(typeof(PackageScope))]
    public sealed class DirectoryWatcherWithPending : IDirectoryWatcherWithPending, IDisposable
    {
        private bool _isPending;

        [NotNull]
        private readonly object _syncRoot = new object();

        [NotNull]
        private readonly Timer _pendingTimer;

        [NotNull]
        private readonly FileSystemWatcher _fileSystemWatcher;

        public Predicate<string> IncludeFilter { get; set; }

        public string Path
        {
            get => _fileSystemWatcher.Path ?? string.Empty;
            set => _fileSystemWatcher.Path = value;
        }

        public int PendingInMilliseconds { get; set; }

        public bool RaiseEvents
        {
            get { return _fileSystemWatcher.EnableRaisingEvents; }
            set
            {
                _fileSystemWatcher.EnableRaisingEvents = value;
                if (value)
                {
                    return;
                }

                lock (_syncRoot)
                {
                    StopPendingTimer();
                }
            }
        }

        public event EventHandler OnDirectoryChanged;

        public DirectoryWatcherWithPending()
        {
            _pendingTimer = new Timer(OnPendingFinished, null, Timeout.Infinite, Timeout.Infinite);
            _fileSystemWatcher = new FileSystemWatcher
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
            };
            _fileSystemWatcher.Changed += OnChanged;
            _fileSystemWatcher.Created += OnChanged;
            _fileSystemWatcher.Deleted += OnChanged;
            _fileSystemWatcher.Renamed += OnChanged;
        }

        private void OnPendingFinished(object state)
        {
            lock (_syncRoot)
            {
                StopPendingTimer();
                //TODO possible deadlock because of Dispatcher.Invoke in handler, which depends on UI thread, which can do StopSolutionTracking in this moment
                OnDirectoryChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnChanged(object sender, [NotNull] FileSystemEventArgs eventArgs)
        {
            ThrowIf.Null(eventArgs, nameof(eventArgs));
            if (_isPending)
            {
                return;
            }

            if (IncludeFilter?.Invoke(eventArgs.FullPath) == false)
            {
                return;
            }

            lock (_syncRoot)
            {
                StartPendingTimer();
            }
        }

        private void StartPendingTimer()
        {
            _isPending = true;
            _pendingTimer.Change(PendingInMilliseconds, Timeout.Infinite);
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
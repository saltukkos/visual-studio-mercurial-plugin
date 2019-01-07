using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [PackageComponent]
    public sealed class SolutionStateTracker : ISolutionStateTracker
    {
        [NotNull]
        [ItemNotNull]
        private static readonly string[] IgnoredPaths = {".hg", ".vs"};

        [NotNull]
        private IReadOnlyList<FileState> _currentFilesState = new FileState[0]; 

        [NotNull]
        private readonly ISourceControlClientFactory _sourceControlClientFactory;

        [NotNull]
        private readonly IDirectoryWatcherWithPending _directoryWatcherWithPending;

        [CanBeNull]
        private ISourceControlClient _currentSourceControlClient;

        public SolutionStateTracker(
            [NotNull] ISourceControlClientFactory sourceControlClientFactory,
            [NotNull] IDirectoryWatcherWithPending directoryWatcherWithPending)
        {
            _sourceControlClientFactory = sourceControlClientFactory;
            _directoryWatcherWithPending = directoryWatcherWithPending;
            _directoryWatcherWithPending.OnDirectoryChanged += OnDirectoryChanged;
            _directoryWatcherWithPending.PendingInMilliseconds = 5000;
            _directoryWatcherWithPending.IncludeFilter =
                path => !IgnoredPaths.Any(
                    ignored => path.Split(Path.DirectorySeparatorChar).Contains(ignored));
        }

        private void OnDirectoryChanged(object sender, EventArgs e)
        {
            _currentFilesState = _currentSourceControlClient.GetNotCleanFiles().ToList();
            Trace.WriteLine($"New files state:\n* {string.Join("\n* ", _currentFilesState.Select(c => c.FilePath))}");
        }

        public void SetActiveSolution(string path)
        {
            if (path == null || !_sourceControlClientFactory.TryCreateClient(path, out var sourceControlClient))
            {
                StopSolutionTracking();
                _currentSourceControlClient = null;
                return;
            }

            _currentSourceControlClient = sourceControlClient;
            StartSolutionTracking();
        }

        private void StartSolutionTracking()
        {
            Debug.Assert(_currentSourceControlClient != null);
            _directoryWatcherWithPending.Path = _currentSourceControlClient.RootPath;
            _directoryWatcherWithPending.RaiseEvents = true;
        }

        private void StopSolutionTracking()
        {
            _directoryWatcherWithPending.RaiseEvents = false;
        }
    }
}
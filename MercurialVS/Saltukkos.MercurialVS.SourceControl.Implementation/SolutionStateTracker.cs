using System;
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
        private const int RefreshPendingInMilliseconds = 200;

        [NotNull]
        [ItemNotNull]
        private static readonly string[] IgnoredPaths = {".hg", ".vs"};

        [NotNull]
        private readonly ISourceControlClientFactory _sourceControlClientFactory;

        [NotNull]
        private readonly IDirectoryWatcherWithPending _directoryWatcherWithPending;

        [NotNull]
        private readonly IDirectoryStateProviderInternal _directoryStateProvider;

        [CanBeNull]
        private ISourceControlClient _currentSourceControlClient;

        public SolutionStateTracker(
            [NotNull] ISourceControlClientFactory sourceControlClientFactory,
            [NotNull] IDirectoryWatcherWithPending directoryWatcherWithPending,
            [NotNull] IDirectoryStateProviderInternal directoryStateProvider)
        {
            _sourceControlClientFactory = sourceControlClientFactory;
            _directoryWatcherWithPending = directoryWatcherWithPending;
            _directoryStateProvider = directoryStateProvider;
            _directoryWatcherWithPending.PendingInMilliseconds = RefreshPendingInMilliseconds;
            _directoryWatcherWithPending.OnDirectoryChanged += OnDirectoryChanged;
            _directoryWatcherWithPending.IncludeFilter = path =>
            {
                return !IgnoredPaths.Any(
                    ignored => path.Split(Path.DirectorySeparatorChar).Contains(ignored));
            };
        }

        private void OnDirectoryChanged(object sender, EventArgs e)
        {
            if (_currentSourceControlClient == null)
            {
                return;
            }

            var allFilesStates = _currentSourceControlClient.GetAllFilesStates();
            _directoryStateProvider.SetNewDirectoryStatus(allFilesStates);
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [Component(typeof(PackageScope))]
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

        [CanBeNull]
        private Task _initializingTask;

        public SolutionStateTracker(
            [NotNull] ISourceControlClientFactory sourceControlClientFactory,
            [NotNull] IDirectoryWatcherWithPending directoryWatcherWithPending,
            [NotNull] IDirectoryStateProviderInternal directoryStateProvider)
        {
            _sourceControlClientFactory = sourceControlClientFactory;
            _directoryWatcherWithPending = directoryWatcherWithPending;
            _directoryStateProvider = directoryStateProvider;
            _directoryWatcherWithPending.PendingInMilliseconds = RefreshPendingInMilliseconds;
            _directoryWatcherWithPending.OnDirectoryChanged += (sender, args) => OnDirectoryChanged();
            _directoryWatcherWithPending.IncludeFilter = path =>
            {
                return !IgnoredPaths.Any(
                    ignored => path.Split(Path.DirectorySeparatorChar).Contains(ignored));
            };
        }

        private void OnDirectoryChanged()
        {
            if (_currentSourceControlClient == null)
            {
                _directoryStateProvider.SetNewDirectoryStatus(new FileState[0]);
                return;
            }

            var allFilesStates = _currentSourceControlClient.GetAllFilesStates();
            _directoryStateProvider.SetNewDirectoryStatus(allFilesStates);
        }

        public void SetActiveSolution(string path)
        {
            if (path == null || !_sourceControlClientFactory.TryCreateClient(path, out var sourceControlClient))
            {
                _currentSourceControlClient = null;
                StopSolutionTracking();
                return;
            }

            _currentSourceControlClient = sourceControlClient;
            StartSolutionTracking();
        }

        private void StartSolutionTracking()
        {
            Debug.Assert(_currentSourceControlClient != null);
            _initializingTask = Task.Run(() =>
            {
                OnDirectoryChanged();
                _directoryWatcherWithPending.Path = _currentSourceControlClient.RootPath;
                _directoryWatcherWithPending.RaiseEvents = true;
                _initializingTask = null;
            });
        }

        private void StopSolutionTracking()
        {
            //TODO cancellation token
            _initializingTask?.Wait();
            _directoryWatcherWithPending.RaiseEvents = false;
            OnDirectoryChanged();
        }
    }
}
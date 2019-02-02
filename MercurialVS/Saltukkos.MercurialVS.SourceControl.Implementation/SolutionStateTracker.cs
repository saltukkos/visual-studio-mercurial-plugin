using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [Component(typeof(SolutionUnderSourceControlScope))]
    public sealed class SolutionStateTracker : IDisposable
    {
        private const int RefreshPendingInMilliseconds = 200;

        [NotNull]
        [ItemNotNull]
        private static readonly string[] IgnoredPaths = {".hg", ".vs"};

        [NotNull]
        private readonly IDirectoryWatcherWithPending _directoryWatcherWithPending;

        [NotNull]
        private readonly IDirectoryStateProviderInternal _directoryStateProvider;

        [NotNull]
        private readonly ISourceControlClient _currentSourceControlClient;

        public SolutionStateTracker(
            [NotNull] SolutionUnderSourceControlInfo solutionUnderSourceControlInfo,
            [NotNull] IDirectoryWatcherWithPending directoryWatcherWithPending,
            [NotNull] IDirectoryStateProviderInternal directoryStateProvider,
            [NotNull] ISourceControlClient currentSourceControlClient)
        {
            _currentSourceControlClient = currentSourceControlClient;
            _directoryStateProvider = directoryStateProvider;

            _directoryWatcherWithPending = directoryWatcherWithPending;
            _directoryWatcherWithPending.PendingInMilliseconds = RefreshPendingInMilliseconds;
            _directoryWatcherWithPending.Path = solutionUnderSourceControlInfo.SourceControlDirectoryPath;
            _directoryWatcherWithPending.OnDirectoryChanged += (sender, args) => OnDirectoryChanged();
            _directoryWatcherWithPending.IncludeFilter = path =>
            {
                ThrowIf.Null(path, nameof(path));
                return !IgnoredPaths.Any(
                    ignored => path.Split(Path.DirectorySeparatorChar).Contains(ignored));
            };
            _directoryWatcherWithPending.RaiseEvents = true;

            OnDirectoryChanged();
        }

        public void Dispose()
        {
            _directoryStateProvider.SetNewDirectoryStatus(new FileState[0]);
            _directoryWatcherWithPending.RaiseEvents = false;
        }

        private void OnDirectoryChanged()
        {
            var allFilesStates = _currentSourceControlClient.GetAllFilesStates();
            _directoryStateProvider.SetNewDirectoryStatus(allFilesStates);
        }
    }
}
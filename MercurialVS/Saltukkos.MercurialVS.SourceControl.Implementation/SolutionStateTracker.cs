using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [Component]
    public sealed class SolutionStateTracker : ISolutionStateTracker
    {
        [NotNull]
        private readonly ISourceControlClientFactory _sourceControlClientFactory;

        [CanBeNull]
        private ISourceControlClient _currentSourceControlClient;

        public SolutionStateTracker([NotNull] ISourceControlClientFactory sourceControlClientFactory)
        {
            _sourceControlClientFactory = sourceControlClientFactory;
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
        }

        private void StopSolutionTracking()
        {
        }
    }
}
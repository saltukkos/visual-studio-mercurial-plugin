using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Component]
    public sealed class MercurialSccProviderService : IMercurialSccProviderService
    {
        [NotNull]
        private readonly ISourceControlLifetimeManager _sourceControlLifetimeManager;

        public MercurialSccProviderService([NotNull] ISourceControlLifetimeManager sourceControlLifetimeManager)
        {
            _sourceControlLifetimeManager = sourceControlLifetimeManager;
        }

        public int SetActive()
        {
            _sourceControlLifetimeManager.StartLifetime();
            return VSConstants.S_OK;
        }

        public int SetInactive()
        {
            _sourceControlLifetimeManager.EndLifetime();
            return VSConstants.S_OK;
        }

        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            pfResult = 0;
            return VSConstants.S_OK;
        }
    }
}
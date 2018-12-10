using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;

namespace StudioIntegrationPackage
{
    [Guid(Constants.SourceControlServiceGuid)]
    public sealed class SccProviderService : IVsSccProvider
    {
        [NotNull]
        private readonly IContainer _container;

        public SccProviderService([NotNull] IContainer container)
        {
            _container = container;
        }

        public int SetActive()
        {
            _container.StartSourceControlLifetime();
            return VSConstants.S_OK;
        }

        public int SetInactive()
        {
            _container.EndSourceControlLifetime();
            return VSConstants.S_OK;
        }

        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            pfResult = 0;
            return VSConstants.S_OK;
        }
    }
}
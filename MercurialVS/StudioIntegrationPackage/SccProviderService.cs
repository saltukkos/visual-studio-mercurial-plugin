using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace StudioIntegrationPackage
{
    [Guid(Constants.SourceControlServiceGuid)]
    public sealed class SccProviderService : IVsSccProvider
    {
        public int SetActive()
        {
            return VSConstants.S_OK;
        }

        public int SetInactive()
        {
            return VSConstants.S_OK;
        }

        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            pfResult = 0;
            return VSConstants.S_OK;
        }
    }
}
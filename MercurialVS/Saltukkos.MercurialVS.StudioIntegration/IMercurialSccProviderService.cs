using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Guid(Constants.SourceControlServiceGuid)]
    public interface IMercurialSccProviderService : IPackageComponent, IVsSccProvider
    {
    }
}
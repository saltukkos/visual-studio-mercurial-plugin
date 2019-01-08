using System.Runtime.InteropServices;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Guid(Constants.SourceControlServiceGuid)]
    public interface IMercurialSccProviderService : IPackageComponent
    {
    }
}
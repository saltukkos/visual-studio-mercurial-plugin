using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace StudioIntegrationPackage
{
    [Guid(Constants.PackageGuid)]
    [ProvideAutoLoad(Constants.PackageGuid)]
    [ProvideService(typeof(SccProviderService))]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [SourceControlRegistration(Constants.PackageGuid, Constants.SourceControlGuid, Constants.SourceControlServiceGuid)]
    public sealed class StudioIntegrationPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
            ;
        }
    }
}

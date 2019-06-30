using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Component(typeof(PackageScope))]
    public class MercurialSccProviderRegister
    {
        public MercurialSccProviderRegister(
            [NotNull] IMercurialSccProviderService mercurialSccProviderService,
            [NotNull] IServiceRegister serviceRegister)
        {
            ThrowIf.Null(serviceRegister, nameof(serviceRegister));
            ThrowIf.Null(mercurialSccProviderService, nameof(mercurialSccProviderService));

            serviceRegister.RegisterService(mercurialSccProviderService);
        }
    }
}
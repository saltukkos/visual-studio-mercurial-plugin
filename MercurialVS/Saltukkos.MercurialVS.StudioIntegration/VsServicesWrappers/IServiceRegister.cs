using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers
{
    public interface IServiceRegister
    {
        void RegisterService<T>([NotNull] T instance);
    }
}
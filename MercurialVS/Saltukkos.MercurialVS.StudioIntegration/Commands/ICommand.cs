using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    public interface ICommand : IPackageComponent
    {
        int CommandId { get; }

        void Invoke();
    }
}
using Saltukkos.Container.Meta;

namespace StudioIntegrationPackage.Commands
{
    public interface ICommand : IPackageComponent
    {
        int CommandId { get; }

        void Invoke();
    }
}
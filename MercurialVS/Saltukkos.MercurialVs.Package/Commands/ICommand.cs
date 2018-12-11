using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.Package.Commands
{
    public interface ICommand : IPackageComponent
    {
        int CommandId { get; }

        void Invoke();
    }
}
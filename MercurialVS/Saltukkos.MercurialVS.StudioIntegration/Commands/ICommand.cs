namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    public interface ICommand
    {
        int CommandId { get; }

        void Invoke();
    }
}
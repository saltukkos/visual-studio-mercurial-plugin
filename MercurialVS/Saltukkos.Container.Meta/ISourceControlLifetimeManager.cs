namespace Saltukkos.Container.Meta
{
    public interface ISourceControlLifetimeManager
    {
        void StartLifetime();

        void EndLifetime();
    }
}
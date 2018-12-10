using JetBrains.Annotations;

namespace Saltukkos.Container.Meta
{
    public interface IContainer
    {
        [NotNull]
        T Resolve<T>();

        void StartSourceControlLifetime();

        void EndSourceControlLifetime();
    }
}
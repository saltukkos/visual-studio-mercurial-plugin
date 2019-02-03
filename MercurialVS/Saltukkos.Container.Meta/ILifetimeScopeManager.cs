using JetBrains.Annotations;

namespace Saltukkos.Container.Meta
{
    public interface ILifetimeScopeManager<TScope, TInitializer>
        where TScope : ILifeTimeScope<TInitializer>
        where TInitializer : class

    {
        void StartScopeLifetime([NotNull] TInitializer initializer);

        void EndScopeLifetime();
    }
}
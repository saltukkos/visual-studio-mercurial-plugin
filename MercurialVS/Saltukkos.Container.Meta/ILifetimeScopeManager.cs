using JetBrains.Annotations;

namespace Saltukkos.Container.Meta
{
    // ReSharper disable once UnusedTypeParameter
    // ReSharper disable once TypeParameterCanBeVariant
    public interface ILifetimeScopeManager<TScope, TInitializer>
        where TScope : ILifeTimeScope<TInitializer>
    {
        void StartScopeLifetime([NotNull] TInitializer initializer);

        void EndScopeLifetime();
    }
}
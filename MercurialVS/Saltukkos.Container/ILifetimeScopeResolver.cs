using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.Container
{
    public interface ILifetimeScopeResolver<TScope, TInitializer> : ILifetimeScopeManager<TScope, TInitializer>
        where TScope : ILifeTimeScope<TInitializer>
    {
        [NotNull]
        T Resolve<T>();
    }
}
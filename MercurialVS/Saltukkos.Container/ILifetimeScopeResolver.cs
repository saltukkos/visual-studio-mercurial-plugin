using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.Container
{
    public interface ILifetimeScopeResolver<TScope> : ILifetimeScopeManager<TScope>
    {
        [NotNull]
        T Resolve<T>();
    }
}
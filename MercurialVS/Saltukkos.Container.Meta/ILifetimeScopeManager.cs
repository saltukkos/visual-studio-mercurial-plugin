using JetBrains.Annotations;
using Saltukkos.Container.Meta.LifetimeScopes;

namespace Saltukkos.Container.Meta
{
    public interface ILifetimeScopeManager<T> where T : PackageScope
    {
        void StartScopeLifetime([NotNull] [ItemNotNull] params object[] additionalComponents);

        void EndScopeLifetime();
    }
}
using JetBrains.Annotations;

namespace Saltukkos.Container.Meta
{
    // ReSharper disable once UnusedTypeParameter
    public interface ILifetimeScopeManager<TScope>
    {
        //TODO constraint on initialization of T-scope
        void StartScopeLifetime([NotNull] [ItemNotNull] params object[] additionalComponents);

        void EndScopeLifetime();
    }
}
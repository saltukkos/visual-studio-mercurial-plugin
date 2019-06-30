using System;
using Autofac;
using JetBrains.Annotations;

namespace Saltukkos.Container
{
    internal delegate void LifetimeDisposer();

    /// Since sometimes we want to start lifetime while other
    /// lifetime expanding (e.g. register callback in constructor and
    /// this callback invoked synchronously for starting nested lifetime)
    /// there is the solution for delayed lifetime starting in case
    /// our parent is expanding now
    internal interface ILifetimeExpandingController
    {
        [NotNull]
        [MustUseReturnValue]
        LifetimeDisposer StartNestedLifetime<TScope>(
            [NotNull] ILifetimeScope parentLifetimeScope,
            [NotNull] Action<Autofac.ContainerBuilder> builder);
    }
}
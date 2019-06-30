using System;
using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.Utils;

namespace Saltukkos.Container
{
    internal sealed class LifetimeScopeManager<TScope, TInitializer> : ILifetimeScopeManager<TScope, TInitializer>
        where TScope : ILifeTimeScope<TInitializer>
    {
        [NotNull]
        private readonly ILifetimeScope _parentLifetimeScope;

        [NotNull]
        [ItemNotNull]
        private readonly IReadOnlyList<Type> _scopedTypes;

        [NotNull]
        private readonly ILifetimeExpandingController _lifetimeExpandingController;

        [CanBeNull]
        private LifetimeDisposer _nestedLifetimeScopeDisposer;

        public LifetimeScopeManager(
            [NotNull] ILifetimeScope parentLifetimeScope,
            [NotNull] [ItemNotNull] IReadOnlyList<Type> scopedTypes,
            [NotNull] ILifetimeExpandingController lifetimeExpandingController)
        {
            ThrowIf.Null(lifetimeExpandingController, nameof(lifetimeExpandingController));
            ThrowIf.Null(parentLifetimeScope, nameof(parentLifetimeScope));
            ThrowIf.Null(scopedTypes, nameof(scopedTypes));
            _parentLifetimeScope = parentLifetimeScope;
            _scopedTypes = scopedTypes;
            _lifetimeExpandingController = lifetimeExpandingController;
        }

        public void StartScopeLifetime(TInitializer scopeInitializer)
        {
            ThrowIf.Null(scopeInitializer, nameof(scopeInitializer));

            if (_nestedLifetimeScopeDisposer != null)
            {
                throw new InvalidOperationException($"Lifetime scope {typeof(TScope)} is already created");
            }

            _nestedLifetimeScopeDisposer = _lifetimeExpandingController.StartNestedLifetime<TScope>(
                _parentLifetimeScope,
                builder =>
                {
                    foreach (var scopedType in _scopedTypes)
                    {
                        builder
                            .RegisterType(scopedType)
                            .AsImplementedInterfaces()
                            .SingleInstance()
                            .AutoActivate();
                    }

                    if (typeof(TInitializer) != typeof(None))
                    {
                        builder
                            .RegisterInstance(scopeInitializer)
                            .AsSelf()
                            .AsImplementedInterfaces();
                    }
                });
        }

        public void EndScopeLifetime()
        {
            if (_nestedLifetimeScopeDisposer is null)
            {
                throw new InvalidOperationException($"Lifetime scope {typeof(TScope)} was not created");
            }

            _nestedLifetimeScopeDisposer.Invoke();
            _nestedLifetimeScopeDisposer = null;
        }
    }
}
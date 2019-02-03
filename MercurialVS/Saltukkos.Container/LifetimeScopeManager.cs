using System;
using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.Utils;

namespace Saltukkos.Container
{
    internal sealed class LifetimeScopeManager<TScope, TInitializer> : ILifetimeScopeResolver<TScope, TInitializer>
        where TScope : ILifeTimeScope<TInitializer>
    {
        [NotNull]
        private readonly ILifetimeScope _parentLifetimeScope;

        [NotNull]
        [ItemNotNull]
        private readonly IReadOnlyList<Type> _scopedTypes;

        [CanBeNull]
        private ILifetimeScope _nestedScope;

        public LifetimeScopeManager([NotNull] ILifetimeScope parentLifetimeScope,
            [NotNull] [ItemNotNull] IReadOnlyList<Type> scopedTypes)
        {
            ThrowIf.Null(parentLifetimeScope, nameof(parentLifetimeScope));
            ThrowIf.Null(scopedTypes, nameof(scopedTypes));
            _parentLifetimeScope = parentLifetimeScope;
            _scopedTypes = scopedTypes;
        }

        public void StartScopeLifetime(TInitializer scopeInitializer)
        {
            ThrowIf.Null(scopeInitializer, nameof(scopeInitializer));
            if (_nestedScope != null)
            {
                throw new InvalidOperationException($"Lifetime scope {typeof(TScope)} is already created");
            }

            _nestedScope = _parentLifetimeScope.BeginLifetimeScope(typeof(TScope), builder =>
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
            if (_nestedScope == null)
            {
                throw new InvalidOperationException($"Lifetime scope {typeof(TScope)} was not created");
            }

            _nestedScope.Dispose();
            _nestedScope = null;
        }

        public T Resolve<T>()
        {
            if (_nestedScope == null)
            {
                throw new InvalidOperationException($"Lifetime scope {typeof(TScope)} was not created");
            }

            return _nestedScope.Resolve<T>();
        }
    }
}
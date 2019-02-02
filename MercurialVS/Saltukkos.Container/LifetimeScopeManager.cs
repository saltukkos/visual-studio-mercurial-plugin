using System;
using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;

namespace Saltukkos.Container
{
    internal sealed class LifetimeScopeManager<TScope> : ILifetimeScopeResolver<TScope>
    {
        [NotNull]
        private readonly ILifetimeScope _parentLifetimeScope;

        [NotNull]
        [ItemNotNull]
        private readonly List<Type> _scopedTypes;

        [CanBeNull]
        private ILifetimeScope _nestedScope;

        public LifetimeScopeManager([NotNull] ILifetimeScope parentLifetimeScope, [NotNull] [ItemNotNull] List<Type> scopedTypes)
        {
            _parentLifetimeScope = parentLifetimeScope;
            _scopedTypes = scopedTypes;
        }

        public void StartScopeLifetime(params object[] additionalComponents)
        {
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

                foreach (var additionalComponent in additionalComponents)
                {
                    builder
                        .RegisterInstance(additionalComponent)
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
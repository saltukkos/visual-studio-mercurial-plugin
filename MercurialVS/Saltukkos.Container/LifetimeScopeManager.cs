using System;
using Autofac;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;

namespace Saltukkos.Container
{
    public class LifetimeScopeManager<T> : ILifetimeScopeManager<T>
        where T : PackageScope
    {
        [NotNull]
        private readonly ILifetimeScope _parentLifetimeScope;

        [CanBeNull]
        private ILifetimeScope _nestedScope;

        public LifetimeScopeManager([NotNull] ILifetimeScope parentLifetimeScope)
        {
            _parentLifetimeScope = parentLifetimeScope;
        }

        public void StartScopeLifetime(params object[] additionalComponents)
        {
            if (_nestedScope != null)
            {
                throw new InvalidOperationException($"Lifetime scope {typeof(T)} is already created");
            }

            _nestedScope = _parentLifetimeScope.BeginLifetimeScope(typeof(T), builder =>
            {
                foreach (var additionalComponent in additionalComponents)
                {
                    builder
                        .RegisterInstance(additionalComponent)
                        .AsImplementedInterfaces()
                        .SingleInstance();
                }
            });
        }

        public void EndScopeLifetime()
        {
            if (_nestedScope == null)
            {
                throw new InvalidOperationException($"Lifetime scope {typeof(T)} was not created");
            }

            _nestedScope.Dispose();
            _nestedScope = null;
        }
    }
}
using System;
using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using AutofacContainer = Autofac.IContainer;
using IContainer = Saltukkos.Container.Meta.IContainer;

namespace Saltukkos.Container
{
    internal sealed class Container : IContainer
    {
        [NotNull]
        [ItemNotNull]
        private readonly IReadOnlyList<Type> _sourceControlComponents;

        [NotNull]
        private readonly AutofacContainer _container;

        [NotNull]
        private readonly ILifetimeScope _packageLifetimeScope;

        [CanBeNull]
        private ILifetimeScope _sourceControlLifetimeScope;

        public Container(
            [ItemNotNull] [NotNull] IReadOnlyList<Type> sourceControlComponents,
            [NotNull] AutofacContainer container)
        {
            _sourceControlComponents = sourceControlComponents;
            _container = container;
            _packageLifetimeScope = container.Resolve<ILifetimeScope>();
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public void StartSourceControlLifetime()
        {
            if (_sourceControlLifetimeScope != null)
            {
                throw new InvalidOperationException("Source control scope is already created");
            }

            _sourceControlLifetimeScope = _packageLifetimeScope.BeginLifetimeScope(builder =>
            {
                foreach (var sourceControlComponent in _sourceControlComponents)
                {
                    builder
                        .RegisterType(sourceControlComponent)
                        .AsImplementedInterfaces()
                        .SingleInstance();
                }
            });
        }

        public void EndSourceControlLifetime()
        {
            if (_sourceControlLifetimeScope == null)
            {
                throw new InvalidOperationException("Source control scope was not started");
            }

            _sourceControlLifetimeScope.Dispose();
            _sourceControlLifetimeScope = null;
        }
    }
}
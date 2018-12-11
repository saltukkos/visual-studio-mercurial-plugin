using System;
using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.Container
{
    public class SourceControlLifetimeManager : ISourceControlLifetimeManager
    {
        [NotNull]
        private readonly ILifetimeScope _packageLifetimeScope;

        [ItemNotNull]
        [NotNull]
        private readonly IReadOnlyList<Type> _sourceControlComponents;

        [CanBeNull]
        private ILifetimeScope _sourceControlLifetimeScope;

        public SourceControlLifetimeManager(
            [NotNull] ILifetimeScope packageLifetimeScope,
            [ItemNotNull] [NotNull] IReadOnlyList<Type> sourceControlComponents)
        {
            _packageLifetimeScope = packageLifetimeScope;
            _sourceControlComponents = sourceControlComponents;
        }

        public void StartLifetime()
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
            _sourceControlLifetimeScope.Resolve<IReadOnlyCollection<ISourceControlComponent>>();
        }

        public void EndLifetime()
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
using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Features.ResolveAnything;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.Utils;

namespace Saltukkos.Container
{
    public sealed class ContainerBuilder
    {
        private const string MyAssembliesPrefix = "Saltukkos.";

        [NotNull]
        private readonly Autofac.ContainerBuilder _containerBuilder = new Autofac.ContainerBuilder();

        [NotNull]
        private readonly ScopesHierarchy _scopesHierarchy;

        [NotNull]
        private readonly ScopeComponentsMap _scopeComponentsMap;

        public ContainerBuilder()
        {
            var scopeComponentsMapBuilder = new ScopeComponentsMapBuilder();
            var scopesHierarchyBuilder = new ScopesHierarchyBuilder();
            ParseAppDomainTypes(scopeComponentsMapBuilder, scopesHierarchyBuilder);
            _scopeComponentsMap = scopeComponentsMapBuilder.Build();
            _scopesHierarchy = scopesHierarchyBuilder.Build();

            _containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
        }

        private static void ParseAppDomainTypes(
            [NotNull] ScopeComponentsMapBuilder scopeComponentsMapBuilder,
            [NotNull] ScopesHierarchyBuilder scopesHierarchyBuilder)
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => x?.FullName.StartsWith(MyAssembliesPrefix) == true)
                .SelectMany(x => x.GetTypes());

            foreach (var type in types)
            {
                var componentAttribute = type.GetCustomAttribute<ComponentAttribute>();
                if (componentAttribute != null)
                {
                    scopeComponentsMapBuilder.AddComponentToScope(type, componentAttribute.ScopeType);
                }

                var scopeAttribute = type.GetCustomAttribute<LifetimeScopeAttribute>();
                if (scopeAttribute != null)
                {
                    scopesHierarchyBuilder.AddScope(type, scopeAttribute.BaseScopeType);
                }
            }
        }

        private void RegisterScopeComponents<TScope, TInitializer>()
            where TScope : ILifeTimeScope<TInitializer>
        {
            var scopeType = typeof(TScope);
            var scopeManagerType = typeof(LifetimeScopeManager<TScope, TInitializer>);
            if (!_scopeComponentsMap.TryGetScopeComponents(scopeType, out var currentScopeTypes))
            {
                return;
            }

            _containerBuilder
                .RegisterType(scopeManagerType)
                .AsImplementedInterfaces()
                .SingleInstance()
                .InstancePerMatchingLifetimeScope(_scopesHierarchy.GetBaseScope(scopeType))
                .WithParameter("scopedTypes", currentScopeTypes);

            foreach (var nestedScope in _scopesHierarchy.GetNestedScopes(scopeType))
            {
                var scopeInterfaceType = nestedScope
                    .GetInterfaces()
                    .Single(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ILifeTimeScope<>));

                // ReSharper disable once PossibleNullReferenceException
                var initializerType = scopeInterfaceType.GetGenericArguments()[0];

                GetType()
                    .GetMethod(nameof(RegisterScopeComponents), BindingFlags.NonPublic | BindingFlags.Instance)?
                    .MakeGenericMethod(nestedScope, initializerType)
                    .Invoke(this, null);
            }
        }

        public void RegisterGlobalComponent<T>([NotNull] T instance) where T : class
        {
            ThrowIf.Null(instance, nameof(instance));
            _containerBuilder
                .RegisterInstance(instance)
                .ExternallyOwned();
        }

        [NotNull]
        public ILifetimeScopeResolver<TRootScope, None> Build<TRootScope>()
            where TRootScope : ILifeTimeScope<None>
        {
            RegisterScopeComponents<TRootScope, None>();
            var container = _containerBuilder.Build(rootTag: typeof(object));
            return container.Resolve<ILifetimeScopeResolver<TRootScope, None>>();
        }
    }
}
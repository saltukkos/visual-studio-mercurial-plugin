using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Features.ResolveAnything;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;

namespace Saltukkos.Container
{
    public class ContainerBuilder
    {
        private const string MyAssembliesPrefix = "Saltukkos.";

        [NotNull]
        private readonly Autofac.ContainerBuilder _containerBuilder = new Autofac.ContainerBuilder();

        public ContainerBuilder()
        {
            _containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            var componentsForEachScope = FindComponentsForEachScope();
            RegisterScopeComponents(typeof(PackageScope), componentsForEachScope);
        }

        [NotNull]
        private static IReadOnlyDictionary<Type, List<Type>> FindComponentsForEachScope()
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => x?.FullName.StartsWith(MyAssembliesPrefix) == true)
                .SelectMany(x => x.GetTypes());

            var components = new Dictionary<Type, List<Type>>();

            foreach (var type in types)
            {
                var componentAttribute = type.GetCustomAttribute<ComponentAttribute>();
                if (componentAttribute is null)
                {
                    continue;
                }

                if (!components.TryGetValue(componentAttribute.ScopeType, out var list))
                {
                    list = new List<Type>();
                    components.Add(componentAttribute.ScopeType, list);
                }

                list.Add(type);
            }

            return components;
        }

        private void RegisterScopeComponents(
            [NotNull] Type scopeType,
            [NotNull] IReadOnlyDictionary<Type, List<Type>> scopeTypes)
        {
            var currentScopeTypes = scopeTypes[scopeType];
            foreach (var type in currentScopeTypes)
            {
                _containerBuilder
                    .RegisterType(type)
                    .AsImplementedInterfaces()
                    .SingleInstance()
                    .InstancePerMatchingLifetimeScope(scopeType);
            }

            var nestedScopes = scopeType.Assembly.GetTypes().Where(type => type.BaseType == scopeType);
            foreach (var nestedScope in nestedScopes)
            {
                var scopeManagerType = typeof(LifetimeScopeManager<>).MakeGenericType(nestedScope);
                _containerBuilder
                    .RegisterType(scopeManagerType)
                    .AsImplementedInterfaces()
                    .SingleInstance()
                    .InstancePerMatchingLifetimeScope(scopeType);

                RegisterScopeComponents(nestedScope, scopeTypes);
            }
        }

        public void RegisterGlobalComponent<T>([NotNull] T instance) where T : class
        {
            _containerBuilder
                .RegisterInstance(instance)
                .ExternallyOwned();
        }

        [NotNull]
        public ILifetimeScopeManager<PackageScope> Build()
        {
            return new LifetimeScopeManager<PackageScope>(_containerBuilder.Build());
        }
    }
}
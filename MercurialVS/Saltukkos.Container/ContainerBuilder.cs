using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Features.ResolveAnything;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.Utils;

namespace Saltukkos.Container
{
    public class ContainerBuilder<TRootScope>
    {
        private const string MyAssembliesPrefix = "Saltukkos.";

        [NotNull]
        private readonly ContainerBuilder _containerBuilder = new ContainerBuilder();

        public ContainerBuilder()
        {
            _containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            var componentsForEachScope = FindComponentsForEachScope();
            RegisterScopeComponents<TRootScope>(componentsForEachScope);
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

        private void RegisterScopeComponents<T>([NotNull] IReadOnlyDictionary<Type, List<Type>> scopeTypes)
        {
            var scopeType = typeof(T);
            var scopeManagerType = typeof(LifetimeScopeManager<T>);
            if (!scopeTypes.TryGetValue(scopeType, out var currentScopeTypes))
            {
                return;
            }

            _containerBuilder
                .RegisterType(scopeManagerType)
                .AsImplementedInterfaces()
                .SingleInstance()
                .InstancePerMatchingLifetimeScope(scopeType.BaseType)
                .WithParameter("scopedTypes", currentScopeTypes);

            var nestedScopes = scopeType.Assembly.GetTypes().Where(type => type.BaseType == scopeType);
            foreach (var nestedScope in nestedScopes)
            {
                GetType()
                    .GetMethod(nameof(RegisterScopeComponents), BindingFlags.NonPublic | BindingFlags.Instance)?
                    .MakeGenericMethod(nestedScope)
                    .Invoke(this, new object[] {scopeTypes});
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
        public ILifetimeScopeResolver<TRootScope> Build()
        {
            var container = _containerBuilder.Build(rootTag: typeof(object));
            return container.Resolve<ILifetimeScopeResolver<TRootScope>>();
        }
    }
}
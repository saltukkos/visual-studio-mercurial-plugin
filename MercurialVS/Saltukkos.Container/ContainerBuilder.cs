using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.Container
{
    public class ContainerBuilder
    {
        private const string MyAssembliesPrefix = "Saltukkos.";

        [NotNull]
        private readonly Autofac.ContainerBuilder _containerBuilder;

        public ContainerBuilder()
        {
            _containerBuilder = new Autofac.ContainerBuilder();

            foreach (var packageComponent in FindInheritors(typeof(IPackageComponent)))
            {
                _containerBuilder
                    .RegisterType(packageComponent)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            var sourceControlComponents = FindInheritors(typeof(ISourceControlComponent)).ToList();
            _containerBuilder
                .Register(context => new SourceControlLifetimeManager(
                    context.Resolve<ILifetimeScope>(),
                    sourceControlComponents))
                .AsImplementedInterfaces()
                .SingleInstance();
        }

        [NotNull]
        [ItemNotNull]
        private static IEnumerable<Type> FindInheritors([NotNull] Type baseTypes)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => x?.FullName.StartsWith(MyAssembliesPrefix) == true)
                .SelectMany(x => x.GetTypes())
                .Where(baseTypes.IsAssignableFrom)
                .Where(type => type?.GetCustomAttribute<ComponentAttribute>() != null);
        }

        public void RegisterGlobalComponent<T>([NotNull] T instance) where T : class
        {
            _containerBuilder
                .RegisterInstance(instance)
                .As<T>()
                ?.ExternallyOwned();
        }

        [NotNull]
        public Container Build()
        {
            return new Container(_containerBuilder.Build());
        }
    }
}
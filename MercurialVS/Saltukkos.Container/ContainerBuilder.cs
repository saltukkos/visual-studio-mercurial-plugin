using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Features.ResolveAnything;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using IContainer = Saltukkos.Container.Meta.IContainer;

namespace Saltukkos.Container
{
    public class ContainerBuilder
    {
        [NotNull]
        private readonly Autofac.ContainerBuilder _containerBuilder;

        [NotNull]
        [ItemNotNull]
        private readonly IReadOnlyList<Type> _sourceControlComponents;

        public ContainerBuilder()
        {
            _containerBuilder = new Autofac.ContainerBuilder();
            _containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            foreach (var packageComponent in FindInheritors(typeof(IPackageComponent)))
            {
                _containerBuilder
                    .RegisterType(packageComponent)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            _sourceControlComponents = FindInheritors(typeof(ISourceControlComponent)).ToList();
        }

        [NotNull]
        [ItemNotNull]
        private static IEnumerable<Type> FindInheritors([NotNull] Type baseTypes)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => x?.FullName.StartsWith("Saltukkos") == true)
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
        public IContainer Build()
        {
            return new Container(_sourceControlComponents, _containerBuilder.Build());
        }
    }
}
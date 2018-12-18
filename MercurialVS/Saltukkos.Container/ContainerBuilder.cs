using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Features.ResolveAnything;
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
            //LoadApplicationAssemblies();

            _containerBuilder = new Autofac.ContainerBuilder();
            _containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

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

        //private void LoadApplicationAssemblies()
        //{
        //    var directoryName = Path.GetDirectoryName(typeof(ContainerBuilder).Assembly.Location)
        //                        ?? throw new NullReferenceException("directory of assembly file");
        //    var assemblies = Directory.GetFiles(directoryName, $"{MyAssembliesPrefix}*.dll");
        //    foreach (var referencedAssembly in assemblies)
        //    {
        //        Assembly.Load(referencedAssembly);
        //    }
        //}

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
                .ExternallyOwned();
        }

        [NotNull]
        public Container Build()
        {
            return new Container(_containerBuilder.Build());
        }
    }
}
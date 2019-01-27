using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Mercurial;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.HgServices.Implementation;
using Saltukkos.MercurialVS.SourceControl.Implementation;
using Saltukkos.MercurialVS.StudioIntegration;
using Saltukkos.Utils;
using Constants = Saltukkos.MercurialVS.StudioIntegration.Constants;

namespace Saltukkos.MercurialVS.Package
{
    [SourceControlRegistration]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(Constants.SourceControlGuid)]
    [ProvideService(typeof(IMercurialSccProviderService))]
    [ProvideOptionPage(typeof(SccProviderOptions),
        Constants.SourceControlCategoryName, Constants.SourceControlProviderName, 0, 0, true)]
    [ProvideToolsOptionsPageVisibility]
    [ProvideToolWindow(typeof(MainToolWindow))]
    [ProvideToolWindowVisibility(typeof(MainToolWindow), Constants.SourceControlGuid)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Constants.PackageGuid)]
    public sealed class StudioIntegrationPackage : Microsoft.VisualStudio.Shell.Package, IToolWindowContainer
    {
        [CanBeNull]
        private Container.Container _container;

        protected override void Initialize()
        {
            try
            {
                base.Initialize();

                _container = BuildContainer();
                _container.Resolve<IReadOnlyCollection<IPackageComponent>>();
                AddService(_container.Resolve<IMercurialSccProviderService>());
                GetService<IVsRegisterScciProvider>()
                    .RegisterSourceControlProvider(Guid.Parse(Constants.SourceControlGuid));
            }
            catch
            {
                Debugger.Break();
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                _container?.Dispose();
            }
            catch
            {
                Debugger.Break();
            }

            base.Dispose(disposing);
        }

        [NotNull]
        private Container.Container BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterGlobalComponent(GetService<IMenuCommandService>());
            containerBuilder.RegisterGlobalComponent(GetService<SVsSolution, IVsSolution>());
            containerBuilder.RegisterGlobalComponent(GetService<SVsSolution, IVsSolution2>());
            containerBuilder.RegisterGlobalComponent(GetService<SVsSolution, IVsHierarchy>());
            containerBuilder.RegisterGlobalComponent(GetService<SOleComponentManager, IOleComponentManager>());
            containerBuilder.RegisterGlobalComponent<IToolWindowContainer>(this);
            var container = containerBuilder.Build();
            return container;
        }

        [NotNull]
        [MustUseReturnValue]
        private TImplemented GetService<TActual, TImplemented>()
        {
            var service = (TImplemented) GetService(typeof(TActual));
            ThrowIf.Null(service, nameof(service));
            return service;
        }

        [NotNull]
        [MustUseReturnValue]
        private T GetService<T>()
        {
            return GetService<T, T>();
        }

        private void AddService<T>([NotNull] T service)
        {
            IServiceContainer container = this;
            container.AddService(typeof(T), service, true);
        }

        #region ReferenceHacks

        static StudioIntegrationPackage()
        {
            Trace.WriteLine(typeof(IRegistryStorage).Assembly.FullName);
            Trace.WriteLine(typeof(SourceControlClientFactory).Assembly.FullName);
            Trace.WriteLine(typeof(Client).Assembly.FullName);
        }

        #endregion
    }
}
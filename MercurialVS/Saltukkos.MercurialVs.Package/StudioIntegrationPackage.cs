using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.StudioIntegration;
using Saltukkos.Utils;
using Constants = Saltukkos.MercurialVS.StudioIntegration.Constants;

namespace Saltukkos.MercurialVS.Package
{
    [SourceControlRegistration]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(Constants.SourceControlGuid)]
    [ProvideService(typeof(IMercurialSccProviderService))]
    [ProvideOptionPage(typeof(SccProviderOptions), "Source Control", Constants.SourceControlProviderName, 0, 0, true)]
    [ProvideToolsOptionsPageVisibility]
    [ProvideToolWindow(typeof(MainToolWindow))]
    [ProvideToolWindowVisibility(typeof(MainToolWindow), Constants.SourceControlGuid)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Constants.PackageGuid)]
    public sealed class StudioIntegrationPackage : Microsoft.VisualStudio.Shell.Package, IToolWindowContainer
    {
        protected override void Initialize()
        {
            base.Initialize();
            var container = BuildContainer();
            container.Resolve<IReadOnlyCollection<IPackageComponent>>();

            AddService(container.Resolve<IMercurialSccProviderService>());
            GetService<IVsRegisterScciProvider>().RegisterSourceControlProvider(Guid.Parse(Constants.SourceControlGuid));
        }

        [NotNull]
        private Container.Container BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterGlobalComponent(GetService<IMenuCommandService>());
            containerBuilder.RegisterGlobalComponent<IToolWindowContainer>(this);
            var container = containerBuilder.Build();
            return container;
        }

        [NotNull]
        [MustUseReturnValue]
        private T GetService<T>()
        {
            var service = (T) GetService(typeof(T));
            ThrowIf.Null(service, nameof(service));
            return service;
        }

        private void AddService<T>([NotNull] T service)
        {
            IServiceContainer container = this;
            container.AddService(typeof(T), service, true);
        }
    }
}
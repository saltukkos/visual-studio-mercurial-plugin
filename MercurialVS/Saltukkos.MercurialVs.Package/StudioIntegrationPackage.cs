using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.StudioIntegration;
using Saltukkos.Utils;
using Constants = Saltukkos.MercurialVS.StudioIntegration.Constants;
using IContainer = Saltukkos.Container.Meta.IContainer;

namespace Saltukkos.MercurialVS.Package
{
    [SourceControlRegistration]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(Constants.SourceControlGuid)]
    [ProvideService(typeof(SccProviderService))]
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
            ((IServiceContainer) this).AddService(typeof(SccProviderService), new SccProviderService(container), true);
            var registerScciProvider = GetService<IVsRegisterScciProvider>();
            registerScciProvider.RegisterSourceControlProvider(Guid.Parse(Constants.SourceControlGuid));
        }

        [NotNull]
        private IContainer BuildContainer()
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
    }
}
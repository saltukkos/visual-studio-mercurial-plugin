using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using JetBrains.Annotations;
using Mercurial;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.HgServices.Implementation;
using Saltukkos.MercurialVS.Package.VsServicesWrappers;
using Saltukkos.MercurialVS.SourceControl.Implementation;
using Saltukkos.MercurialVS.StudioIntegration;
using Saltukkos.MercurialVS.StudioIntegration.FileHistory;
using Saltukkos.MercurialVS.StudioIntegration.SolutionFilesStatus;
using Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers;
using Saltukkos.Utils;
using Constants = Saltukkos.MercurialVS.StudioIntegration.Constants;
using Debugger = System.Diagnostics.Debugger;

namespace Saltukkos.MercurialVS.Package
{
    [SourceControlRegistration]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(Constants.SourceControlGuid)]
    [ProvideService(typeof(IMercurialSccProviderService))]
    [ProvideOptionPage(
        typeof(SccProviderOptions),
        Constants.SourceControlCategoryName, 
        Constants.SourceControlProviderName,
        0, 0, true)]
    [ProvideToolsOptionsPageVisibility]
    [ProvideToolWindow(
        typeof(SolutionFilesStatusToolWindow),
        Window = "{3AE79031-E1BC-11D0-8F78-00A0C9110057}", //EnvDTE.Constants.vsWindowKindSolutionExplorer
        Style = VsDockStyle.Tabbed)]
    [ProvideToolWindow(
        typeof(FileHistoryToolWindow),
        Style = VsDockStyle.MDI,
        MultiInstances = true)]
    [ProvideToolWindowVisibility(typeof(SolutionFilesStatusToolWindow), Constants.SourceControlGuid)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Constants.PackageGuid)]
    public sealed class StudioIntegrationPackage : Microsoft.VisualStudio.Shell.Package, IToolWindowContainer
    {
        [CanBeNull]
        private ILifetimeScopeManager<PackageScope, None> _rootLifetimeManager;

        protected override void Initialize()
        {
            try
            {
                base.Initialize();

                _rootLifetimeManager = BuildContainer();
                _rootLifetimeManager.StartScopeLifetime(new None());

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
                _rootLifetimeManager?.EndScopeLifetime();
            }
            catch
            {
                Debugger.Break();
            }

            base.Dispose(disposing);
        }

        [NotNull]
        private ILifetimeScopeManager<PackageScope, None> BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterGlobalComponent(new DteWrapper(GetService<DTE>()) as IDte);
            containerBuilder.RegisterGlobalComponent(new PackageWrapper(this) as IToolWindowContainer);
            containerBuilder.RegisterGlobalComponent(new PackageWrapper(this) as IServiceRegister);
            containerBuilder.RegisterGlobalComponent(GetService<IMenuCommandService>());
            containerBuilder.RegisterGlobalComponent(GetService<IVsRegisterScciProvider>());
            containerBuilder.RegisterGlobalComponent(GetService<SVsSolution, IVsSolution>());
            containerBuilder.RegisterGlobalComponent(GetService<SVsSolution, IVsSolution2>());
            containerBuilder.RegisterGlobalComponent(GetService<SVsSolution, IVsHierarchy>());
            containerBuilder.RegisterGlobalComponent(GetService<SOleComponentManager, IOleComponentManager>());
            containerBuilder.RegisterGlobalComponent(GetService<SVsDifferenceService, IVsDifferenceService>());
            containerBuilder.RegisterGlobalComponent(GetService<SVsFileMergeService, IVsFileMergeService>());
            containerBuilder.RegisterGlobalComponent(GetService<SVsUIShellOpenDocument, IVsUIShellOpenDocument>());
            var lifetimeScopeResolver = containerBuilder.Build<PackageScope>();
            return lifetimeScopeResolver;
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

        #region ReferenceHacks

        static StudioIntegrationPackage()
        {
            Trace.WriteLine(typeof(IRegistryStorage).Assembly.FullName);
            Trace.WriteLine(typeof(SourceControlBasePathProvider).Assembly.FullName);
            Trace.WriteLine(typeof(IClient).Assembly.FullName);
        }

        #endregion
    }
}
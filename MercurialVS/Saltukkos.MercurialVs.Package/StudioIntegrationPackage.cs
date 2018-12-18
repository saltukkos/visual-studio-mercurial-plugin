using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Saltukkos.Container;
using Saltukkos.Container.Meta;
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
    [ProvideOptionPage(typeof(SccProviderOptions), Constants.SourceControlCategoryName, Constants.SourceControlProviderName, 0, 0, true)]
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
                AddComponentToPagesAndProfiles(_container.Resolve<SccProviderOptions>());

                //TODO load assemblies
                var t = typeof(ConfigurationStorage).FullName;

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
            try {
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
            containerBuilder.RegisterGlobalComponent<IToolWindowContainer>(this);
            containerBuilder.RegisterGlobalComponent(new ShellSettingsManager(this));
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

        //TODO dirty hack, but fixes bad design of optionsPages
        private void AddComponentToPagesAndProfiles([NotNull] object component)
        {
            var pagesAndProfilesField = GetType().BaseType?.GetField("_pagesAndProfiles", BindingFlags.Instance | BindingFlags.NonPublic);
            ThrowIf.Null(pagesAndProfilesField, nameof(pagesAndProfilesField));
            var pagesAndProfiles = (System.ComponentModel.Container) pagesAndProfilesField.GetValue(this);
            if (pagesAndProfiles == null)
            {
                var type = GetType().BaseType?
                    .GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)
                    .Single(x => x.FullName?.Contains("Container") == true);

                var constructorInfo = type
                    ?.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Single();
                
                ThrowIf.Null(constructorInfo, nameof(constructorInfo));
                
                pagesAndProfiles = (System.ComponentModel.Container)constructorInfo.Invoke(new object[] { this });
                pagesAndProfilesField.SetValue(this, pagesAndProfiles);
            }

            pagesAndProfiles?.Add((IComponent) component);
        }
    }
}
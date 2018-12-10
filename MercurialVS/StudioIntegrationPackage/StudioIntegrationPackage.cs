using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace StudioIntegrationPackage
{
    [SourceControlRegistration]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(Constants.SourceControlGuid)]
    [ProvideService(typeof(SccProviderService))]
    [ProvideOptionPage(typeof(SccProviderOptions), "Source Control", Constants.SourceControlProviderName, 0, 0, true)]
    //TODO ProvideOptionsPageVisibility
    [ProvideToolWindow(typeof(MainToolWindow))]
    [ProvideToolWindowVisibility(typeof(MainToolWindow), Constants.SourceControlGuid)]
    [Guid(Constants.PackageGuid)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class StudioIntegrationPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            ((IServiceContainer) this).AddService(typeof(SccProviderService), new SccProviderService(), true);

            var registerScciProvider = GetService<IVsRegisterScciProvider>();
            registerScciProvider.RegisterSourceControlProvider(Guid.Parse(Constants.SourceControlGuid));

            var menuCommandService = GetService<IMenuCommandService>();
            var menuCommand =
                new MenuCommand((sender, args) =>
                    {
                        ((IVsWindowFrame) MainToolWindow.CreatedInstance?.Frame)?.Show();
                    },
                    new CommandID(Guid.Parse(Constants.CommandSetGuid), Constants.ShowToolWindowCommandId));

            menuCommandService.AddCommand(menuCommand);
        }

        [NotNull]
        [MustUseReturnValue]
        private T GetService<T>()
        {
            var service = (T) GetService(typeof(T));
            if (service == null)
            {
                throw new InvalidEnumArgumentException($@"No such service {typeof(T)}");
            }

            return service;
        }
    }
}
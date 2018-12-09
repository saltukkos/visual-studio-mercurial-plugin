using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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
    //TODO menus, commands
    [ProvideToolWindow(typeof(MainToolWindow))]
    [ProvideToolWindowVisibility(typeof(MainToolWindow), Constants.SourceControlGuid)]
    [Guid(Constants.PackageGuid)]
    public sealed class StudioIntegrationPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            ((IServiceContainer)this).AddService(typeof(SccProviderService), new SccProviderService(), true);

            var registerScciProvider = GetService<IVsRegisterScciProvider>();
            registerScciProvider.RegisterSourceControlProvider(Guid.Parse(Constants.SourceControlGuid));

            var menuCommandService = (OleMenuCommandService)GetService<IMenuCommandService>();
            var menuCommand =
                new OleMenuCommand((sender, args) => MessageBox.Show("dsd"), new CommandID(Guid.NewGuid(), 1))
                {
                    Text = "Sample text"
                };
            var oleCommandTarget = menuCommandService.ParentTarget;
            menuCommandService.AddCommand(menuCommand);
        }

        [NotNull]
        [MustUseReturnValue]
        private T GetService<T>()
        {
            var service = (T)GetService(typeof(T));
            if (service == null)
            {
                throw new InvalidEnumArgumentException($@"No such service {typeof(T)}");
            }

            return service;
        }
    }
}

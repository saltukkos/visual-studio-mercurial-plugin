using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace StudioIntegrationPackage
{
    [SourceControlRegistration]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(Constants.SourceControlGuid)]
    [ProvideService(typeof(SccProviderService))]
    [ProvideOptionPage(typeof(SccProviderOptions), "Source Control", Constants.SourceControlProviderName, 0, 0, true)]
    //TODO ProvideToolsOptionsPageVisibility
    //TODO ToolWindow
    //TODO menus, commands
    [Guid(Constants.PackageGuid)]
    public sealed class StudioIntegrationPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            ((IServiceContainer)this).AddService(typeof(SccProviderService), new SccProviderService(), true);

            var registerScciProvider = (IVsRegisterScciProvider)GetService(typeof(IVsRegisterScciProvider));
            registerScciProvider.RegisterSourceControlProvider(Guid.Parse(Constants.SourceControlGuid));

        }
    }
}

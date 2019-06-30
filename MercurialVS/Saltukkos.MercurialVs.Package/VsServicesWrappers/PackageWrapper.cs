using System;
using System.ComponentModel.Design;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.Package.VsServicesWrappers
{
    internal class PackageWrapper : IToolWindowContainer, IServiceRegister
    {
        [NotNull]
        private readonly Microsoft.VisualStudio.Shell.Package _package;

        public PackageWrapper([NotNull] Microsoft.VisualStudio.Shell.Package package)
        {
            ThrowIf.Null(package, nameof(package));
            _package = Ensure.NotNull(package);
        }

        public ToolWindowPane FindToolWindow(Type toolWindowType, int id, bool create) =>
            _package.FindToolWindow(toolWindowType, id, create);

        public void RegisterService<T>(T instance)
        {
            ((IServiceContainer) _package).AddService(typeof(T), instance, true);
        }
    }
}
using EnvDTE;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.Package.VsServicesWrappers
{
    internal class DteWrapper : IDte
    {
        [NotNull]
        private readonly DTE _dte;

        public DteWrapper([NotNull] DTE dte)
        {
            ThrowIf.Null(dte, nameof(dte));
            _dte = dte;
        }

        public Document ActiveDocument => _dte.ActiveDocument;
    }

}
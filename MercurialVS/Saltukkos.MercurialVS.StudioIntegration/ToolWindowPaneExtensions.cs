using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    internal static class ToolWindowPaneExtensions
    {
        public static void Show([NotNull] this ToolWindowPane toolWindowPane)
        {
            ThrowIf.Null(toolWindowPane, nameof(toolWindowPane));
            ((IVsWindowFrame)toolWindowPane.Frame)?.Show();
        }
    }
}
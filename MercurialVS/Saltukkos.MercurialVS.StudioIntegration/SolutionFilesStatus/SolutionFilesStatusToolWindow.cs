using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.SolutionFilesStatus
{
    [Guid(Constants.SolutionFilesStatusToolWindowGuid)]
    public class SolutionFilesStatusToolWindow : ToolWindowPane
    {
        [NotNull]
        private readonly ElementHost _elementHost;

        [NotNull]
        private readonly SolutionFilesStatusViewModel _solutionFilesStatusViewModel;

        public SolutionFilesStatusToolWindow()
        {
            Caption = "VCS changes";

            var dependenciesProvider = ToolWindowsDependenciesProvider.GetInstance();
            _solutionFilesStatusViewModel = Ensure.NotNull(dependenciesProvider.SolutionFilesStatusViewModelFactoryFunc.Invoke());

            using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
            {
                _elementHost = new ElementHost
                {
                    Child = new PendingChangesView(_solutionFilesStatusViewModel),
                    Dock = DockStyle.Fill
                };
            }
        }

        public override IWin32Window Window => _elementHost;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _elementHost.Dispose();
                _solutionFilesStatusViewModel.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
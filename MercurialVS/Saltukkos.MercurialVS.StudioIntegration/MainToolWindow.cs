using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Saltukkos.MercurialVS.StudioIntegration.SolutionFilesStatus;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Guid(Constants.ToolWindowPaneGuid)]
    public class MainToolWindow : ToolWindowPane
    {
        [NotNull]
        private readonly ElementHost _elementHost;

        [NotNull]
        private readonly SolutionFilesStatusViewModel _solutionFilesStatusViewModel;

        public MainToolWindow()
        {
            Caption = "VCS changes";

            var dependenciesProvider = ToolWindowsDependenciesProvider.GetInstance();
            _solutionFilesStatusViewModel = dependenciesProvider.SolutionFilesStatusViewModelFactoryFunc.Invoke();

            _elementHost = new ElementHost
            {
                Child = new PendingChangesView(_solutionFilesStatusViewModel),
                Dock = DockStyle.Fill
            };
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
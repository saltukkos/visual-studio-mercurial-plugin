using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;

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
            Caption = "Main tool window";

            var dependenciesProvider = ToolWindowsDependenciesProvider.GetInstance();
            _solutionFilesStatusViewModel = new SolutionFilesStatusViewModel(
                dependenciesProvider.DirectoryStateProvider,
                dependenciesProvider.VsDifferenceService,
                dependenciesProvider.FileHistoryProvider);

            _elementHost = new ElementHost
            {
                Child = new PendingChangesView(_solutionFilesStatusViewModel),
                Dock = DockStyle.Fill
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _elementHost.Dispose();
                _solutionFilesStatusViewModel.Dispose();
            }

            base.Dispose(disposing);
        }

        public override IWin32Window Window => _elementHost;
    }
}
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    [Guid(Constants.FileHistoryToolWindowGuid)]
    public class FileHistoryToolWindow : ToolWindowPane
    {
        [NotNull]
        private readonly ElementHost _elementHost;

        [NotNull]
        private readonly FileHistoryViewModel _fileHistoryViewModel = new FileHistoryViewModel();

        public FileHistoryToolWindow()
        {
            //TODO dynamic name
            Caption = "File history";

            _elementHost = new ElementHost
            {
                Child = new FileHistoryView(_fileHistoryViewModel),
                Dock = DockStyle.Fill
            };
        }

        public override IWin32Window Window => _elementHost;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _elementHost.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
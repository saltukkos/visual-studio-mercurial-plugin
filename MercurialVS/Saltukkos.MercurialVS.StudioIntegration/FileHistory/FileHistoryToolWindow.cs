using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    [Guid(Constants.FileHistoryToolWindowGuid)]
    public class FileHistoryToolWindow : ToolWindowPane
    {
        [NotNull]
        private readonly ElementHost _elementHost;

        public FileHistoryToolWindow()
        {
            Caption = "File history";

            using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
            {
                _elementHost = new ElementHost
                {
                    Dock = DockStyle.Fill
                };
            }
        }

        public override IWin32Window Window => _elementHost;

        public void SetFileViewModel([NotNull] string fullName, [NotNull] FileHistoryViewModel fileHistoryViewModel)
        {
            ThrowIf.Null(fullName, nameof(fullName));
            ThrowIf.Null(fileHistoryViewModel, nameof(fileHistoryViewModel));

            Caption = $"History - {Path.GetFileName(fullName)}";
            _elementHost.Child = new FileHistoryView(fileHistoryViewModel);
        }

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
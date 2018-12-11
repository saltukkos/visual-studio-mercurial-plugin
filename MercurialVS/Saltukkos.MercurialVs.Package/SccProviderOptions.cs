using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace StudioIntegrationPackage
{
    [Guid(Constants.OptionsPageGuid)]
    public sealed class SccProviderOptions : DialogPage
    {
        private SccProviderOptionsControl _page;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window
        {
            get
            {
                _page = new SccProviderOptionsControl();
                _page.Location = new Point(0, 0);
                return _page;
            }
        }
    }
}
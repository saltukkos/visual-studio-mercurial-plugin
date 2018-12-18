using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Guid(Constants.OptionsPageGuid)]
    public sealed class SccProviderOptions : DialogPage
    {

        public SccProviderOptions()
        {
            _page = SccProviderOptionsControl.Instance ??
                    throw new ArgumentNullException(nameof(SccProviderOptionsControl.Instance));
        }

        [NotNull]
        private SccProviderOptionsControl _page;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window => _page;

        public override void LoadSettingsFromStorage()
        {
            _page.LoadConfiguration();
        }

        public override void SaveSettingsToStorage()
        {
            _page.StoreConfiguration();
        }
    }
}
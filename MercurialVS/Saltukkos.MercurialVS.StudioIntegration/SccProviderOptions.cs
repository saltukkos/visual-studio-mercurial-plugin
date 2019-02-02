using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Guid(Constants.OptionsPageGuid)]
    public sealed class SccProviderOptions : DialogPage
    {
        [NotNull]
        private readonly SccProviderOptionsControl _optionsControl;

        public SccProviderOptions()
        {
            var dependencies = ToolWindowsDependenciesProvider.GetInstance();
            _optionsControl = new SccProviderOptionsControl(dependencies.ConfigurationStorage);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window => _optionsControl;

        public override void LoadSettingsFromStorage()
        {
            _optionsControl.LoadConfiguration();
        }

        public override void SaveSettingsToStorage()
        {
            _optionsControl.StoreConfiguration();
        }
    }
}
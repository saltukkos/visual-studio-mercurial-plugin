using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.SourceControl;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    public partial class SccProviderOptionsControl : UserControl
    {
        [NotNull]
        private readonly IConfigurationStorage _configurationStorage;

        public SccProviderOptionsControl([NotNull] IConfigurationStorage configurationStorage)
        {
            _configurationStorage = configurationStorage;
            InitializeComponent();
        }

        public void LoadConfiguration()
        {
            checkBox1.Checked = _configurationStorage.SomeFlag;
            textBox1.Text = _configurationStorage.SomeString;
        }

        public void StoreConfiguration()
        {
            _configurationStorage.SomeFlag = checkBox1.Checked;
            _configurationStorage.SomeString = textBox1.Text ?? string.Empty;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // ReSharper disable once LocalizableElement
            MessageBox.Show("Clicked!");
        }
    }
}

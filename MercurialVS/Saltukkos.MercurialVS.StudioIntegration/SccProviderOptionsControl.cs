using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.SourceControl;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Component]
    public partial class SccProviderOptionsControl : UserControl, IPackageComponent
    {
        [NotNull]
        private readonly IConfigurationStorage _configurationStorage;

        [CanBeNull]
        public static SccProviderOptionsControl Instance { get; private set; }

        public SccProviderOptionsControl([NotNull] IConfigurationStorage configurationStorage)
        {
            _configurationStorage = configurationStorage;
            //because of bad designed ProviderOption creation trough parameterless constructor
            Instance = this;
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
            MessageBox.Show("Clicked!");
        }
    }
}

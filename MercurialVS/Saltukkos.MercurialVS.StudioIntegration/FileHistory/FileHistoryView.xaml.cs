using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    /// <summary>
    /// Interaction logic for FileHistoryView.xaml
    /// </summary>
    public partial class FileHistoryView
    {
        public FileHistoryView([NotNull] FileHistoryViewModel viewModel)
        {
            ThrowIf.Null(viewModel, nameof(viewModel));
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

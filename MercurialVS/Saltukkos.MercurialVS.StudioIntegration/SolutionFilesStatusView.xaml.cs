using System.Windows.Input;
using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    /// <summary>
    /// Interaction logic for SolutionFilesStatusControl.xaml
    /// </summary>
    public partial class SolutionFilesStatusView
    {
        [NotNull]
        private readonly SolutionFilesStatusViewModel _viewModel;

        public SolutionFilesStatusView([NotNull] SolutionFilesStatusViewModel viewModel)
        {
            ThrowIf.Null(viewModel, nameof(viewModel));
            DataContext = _viewModel = viewModel;
            InitializeComponent();
        }

        private void OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _viewModel.OnItemClicked();
        }
    }
}
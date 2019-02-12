using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.StudioIntegration.SolutionFilesStatus
{
    public partial class PendingChangesView
    {
        [NotNull]
        private readonly SolutionFilesStatusViewModel _viewModel;

        public PendingChangesView([NotNull] SolutionFilesStatusViewModel viewModel)
        {
            DataContext = _viewModel = viewModel;
            InitializeComponent();
        }

        private void ShowDiff(object sender, RoutedEventArgs e)
        {
            _viewModel.OnItemClicked();
        }

        private void OpenSelectedFile(object sender, RoutedEventArgs e)
        {
            if (!(sender is ListViewItem item) || !item.IsSelected)
            {
                return;
            }

            _viewModel.OnItemClicked();
        }

        private void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.OnItemClicked();
            }
        }
    }
}
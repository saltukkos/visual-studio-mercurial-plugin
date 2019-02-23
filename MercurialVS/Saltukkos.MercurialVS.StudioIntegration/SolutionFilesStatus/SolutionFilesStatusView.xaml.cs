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

        //TODO routed commands

        private void ShowDiff(object sender, RoutedEventArgs e)
        {
            _viewModel.OnDiffClicked();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            _viewModel.OnOpenClicked();
        }

        private void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.OnItemClicked();
            }
        }

        private void OnItemClicked(object sender, MouseButtonEventArgs e)
        {
            _viewModel.OnItemClicked();
        }

        private void OnContextMenuLoaded(object sender, RoutedEventArgs e)
        {
            ((ContextMenu) sender).DataContext = _viewModel;
        }
    }
}
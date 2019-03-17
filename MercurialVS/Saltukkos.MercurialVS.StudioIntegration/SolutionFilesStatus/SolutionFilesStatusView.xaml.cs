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

        private void ShowDiff([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            _viewModel.OnDiffClicked();
        }

        private void OpenFile([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            _viewModel.OnOpenClicked();
        }

        private void OnListViewKeyDown([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.OnItemClicked();
            }
        }

        private void OnItemClicked([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            _viewModel.OnItemClicked();
        }

        private void OnContextMenuLoaded([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            ((ContextMenu) sender).DataContext = _viewModel;
        }
    }
}
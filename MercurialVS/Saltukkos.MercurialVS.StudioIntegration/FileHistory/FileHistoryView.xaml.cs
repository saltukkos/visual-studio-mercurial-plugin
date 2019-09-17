using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    /// <summary>
    /// Interaction logic for FileHistoryView.xaml
    /// </summary>
    public partial class FileHistoryView
    {
        [NotNull]
        private readonly FileHistoryViewModel _viewModel;

        public FileHistoryView([NotNull] FileHistoryViewModel viewModel)
        {
            ThrowIf.Null(viewModel, nameof(viewModel));
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void OnContextMenuLoaded([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            ((ContextMenu)sender).DataContext = _viewModel;
        }

        private void DiffToParent(object sender, RoutedEventArgs e)
        {
            _viewModel.ShowDiffToParent();
        }

        private void DiffToLocal(object sender, RoutedEventArgs e)
        {
            _viewModel.ShowDiffToLocal();
        }

        private void ViewAtRevision(object sender, RoutedEventArgs e)
        {
            _viewModel.ViewAtRevision();
        }
    }
}

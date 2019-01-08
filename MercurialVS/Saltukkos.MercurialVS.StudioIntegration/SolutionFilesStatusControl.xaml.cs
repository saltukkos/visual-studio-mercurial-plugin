using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    /// <summary>
    /// Interaction logic for SolutionFilesStatusControl.xaml
    /// </summary>
    public partial class SolutionFilesStatusControl
    {
        public ObservableCollection<FileState> Files { get; } = new ObservableCollection<FileState>();

        public SolutionFilesStatusControl()
        {
            DataContext = this;
            InitializeComponent();
        }

        public void SetFiles([NotNull] IEnumerable<FileState> files)
        {
            Dispatcher.Invoke(() =>
            {
                Files.Clear();
                foreach (var file in files)
                {
                    Files.Add(file);
                }
            }, DispatcherPriority.Normal);
        }
    }
}
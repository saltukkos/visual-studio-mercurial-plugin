using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    public class FileHistoryViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChangeSet> ChangeSets { get; } = new ObservableCollection<ChangeSet>();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
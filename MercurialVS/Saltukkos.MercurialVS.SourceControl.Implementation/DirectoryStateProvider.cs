using System;
using System.Collections.Generic;
using System.Linq;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [PackageComponent]
    internal class DirectoryStateProvider : IDirectoryStateProviderInternal
    {
        public event EventHandler DirectoryStateChanged;

        public IReadOnlyList<FileState> CurrentStatus { get; private set; }

        public DirectoryStateProvider()
        {
            CurrentStatus = new List<FileState>();
        }

        public void SetNewDirectoryStatus(IEnumerable<FileState> fileStatuses)
        {
            CurrentStatus = fileStatuses.ToList();
            DirectoryStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [Component(typeof(PackageScope))]
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
            ThrowIf.Null(fileStatuses, nameof(fileStatuses));
            CurrentStatus = fileStatuses.ToList();
            DirectoryStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
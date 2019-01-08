using System.Collections.Generic;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    public interface IDirectoryStateProviderInternal : IDirectoryStateProvider
    {
        void SetNewDirectoryStatus([NotNull] IEnumerable<FileState> fileStatuses);
    }
}
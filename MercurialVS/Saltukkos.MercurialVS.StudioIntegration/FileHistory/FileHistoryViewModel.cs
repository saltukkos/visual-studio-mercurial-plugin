using System.Collections.Generic;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    public class FileHistoryViewModel
    {
        public FileHistoryViewModel([NotNull] IReadOnlyList<ChangeSet> changeSets)
        {
            ThrowIf.Null(changeSets, nameof(changeSets));
            ChangeSets = changeSets;
        }

        [NotNull] 
        public IReadOnlyList<ChangeSet> ChangeSets { get; }
    }
}
using System.Collections.Generic;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    public sealed class FileHistoryInfo
    {
        public FileHistoryInfo([NotNull] string filePath, [NotNull] IReadOnlyList<ChangeSet> changeSets)
        {
            ThrowIf.Null(changeSets, nameof(changeSets));
            ThrowIf.Null(filePath, nameof(filePath));
            FilePath = filePath;
            ChangeSets = changeSets;
        }

        [NotNull] public string FilePath { get; }

        [NotNull] public IReadOnlyList<ChangeSet> ChangeSets { get; }
    }
}
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.HgServices
{
    public interface ISourceControlClient
    {
        [NotNull]
        string RootPath { get; }

        [NotNull]
        IReadOnlyList<FileState> GetNotCleanFiles();

        [NotNull]
        IReadOnlyList<FileState> GetAllFilesStates();

        [NotNull]
        string GetFileAtCurrentRevision(string filename);
    }
}

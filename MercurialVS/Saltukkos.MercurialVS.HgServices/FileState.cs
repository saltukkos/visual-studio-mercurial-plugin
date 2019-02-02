using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.HgServices
{
    public struct FileState
    {
        public FileState([NotNull] string filePath, FileStatus status)
        {
            ThrowIf.Null(filePath, nameof(filePath));
            FilePath = filePath;
            Status = status;
        }

        [NotNull]
        public string FilePath { get; }

        public FileStatus Status { get; }
    }
}
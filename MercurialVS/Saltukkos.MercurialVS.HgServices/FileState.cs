using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.HgServices
{
    public struct FileState
    {
        public FileState([NotNull] string filePath, FileStatus status, [NotNull] string relativePath)
        {
            ThrowIf.Null(relativePath, nameof(relativePath));
            ThrowIf.Null(filePath, nameof(filePath));
            FilePath = filePath;
            Status = status;
            RelativePath = relativePath;
        }

        [NotNull]
        public string FilePath { get; }

        [NotNull]
        public string RelativePath { get; }

        public FileStatus Status { get; }
    }
}
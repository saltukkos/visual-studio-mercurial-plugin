using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.HgServices
{
    public struct FileState
    {
        public FileState([NotNull] string filePath, FileStatus status)
        {
            FilePath = filePath;
            Status = status;
        }

        [NotNull]
        public string FilePath { get; }

        public FileStatus Status { get; }
    }
}
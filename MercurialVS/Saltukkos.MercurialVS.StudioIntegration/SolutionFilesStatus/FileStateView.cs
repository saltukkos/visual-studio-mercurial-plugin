using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.StudioIntegration.SolutionFilesStatus
{
    public struct FileStateView
    {
        [NotNull]
        public string FileName { get; set; }

        [NotNull]
        public string RelativePath { get; set; }

        [NotNull]
        public string FullPath { get; set; }

        public FileStatus Status { get; set; }
    }
}
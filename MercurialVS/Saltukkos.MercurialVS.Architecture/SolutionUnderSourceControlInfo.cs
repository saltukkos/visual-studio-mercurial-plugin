using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.Architecture
{
    public class SolutionUnderSourceControlInfo
    {
        public SolutionUnderSourceControlInfo(
            [NotNull] string solutionDirectoryPath,
            [NotNull] string sourceControlDirectoryPath)
        {
            ThrowIf.Null(solutionDirectoryPath, nameof(solutionDirectoryPath));
            ThrowIf.Null(sourceControlDirectoryPath, nameof(sourceControlDirectoryPath));
            SolutionDirectoryPath = solutionDirectoryPath;
            SourceControlDirectoryPath = sourceControlDirectoryPath;
        }

        [NotNull]
        public string SolutionDirectoryPath { get; }

        [NotNull]
        public string SourceControlDirectoryPath { get; }
    }
}
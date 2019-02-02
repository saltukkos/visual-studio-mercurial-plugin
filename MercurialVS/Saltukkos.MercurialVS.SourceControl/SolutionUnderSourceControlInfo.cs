using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.SourceControl
{
    //TODO move to infrastructure package
    public class SolutionUnderSourceControlInfo
    {
        public SolutionUnderSourceControlInfo(
            [NotNull] string solutionDirectoryPath,
            [NotNull] string sourceControlDirectoryPath)
        {
            SolutionDirectoryPath = solutionDirectoryPath;
            SourceControlDirectoryPath = sourceControlDirectoryPath;
        }

        [NotNull]
        public string SolutionDirectoryPath { get; }

        [NotNull]
        public string SourceControlDirectoryPath { get; }
    }
}
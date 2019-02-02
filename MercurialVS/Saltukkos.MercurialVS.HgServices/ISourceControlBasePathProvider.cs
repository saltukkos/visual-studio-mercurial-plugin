using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.HgServices
{
    public interface ISourceControlBasePathProvider
    {
        [ContractAnnotation("=> true, basePath: notnull; => false, basePath: null")]
        bool TryGetBasePath([NotNull] string solutionPath, out string basePath);
    }
}
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.HgServices
{
    public interface ISourceControlClientFactory
    {
        [ContractAnnotation("=> true, client: notnull; => false, client: null")]
        bool TryCreateClient([NotNull] string solutionPath, out ISourceControlClient client);
    }
}
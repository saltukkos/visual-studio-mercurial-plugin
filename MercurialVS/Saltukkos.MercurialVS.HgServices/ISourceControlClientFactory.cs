using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.HgServices
{
    public interface ISourceControlClientFactory : IPackageComponent
    {
        [ContractAnnotation("=> true, client: notnull; => false, client: null")]
        bool TryCreateClient([NotNull] string solutionPath, out ISourceControlClient client);
    }
}
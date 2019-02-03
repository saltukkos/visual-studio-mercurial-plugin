using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.Architecture
{
    [LifetimeScope(typeof(SourceControlScope))]
    public class SolutionUnderSourceControlScope : ILifeTimeScope<SolutionUnderSourceControlInfo>
    {

    }
}
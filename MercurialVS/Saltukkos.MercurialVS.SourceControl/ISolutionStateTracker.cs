using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.SourceControl
{
    public interface ISolutionStateTracker : IPackageComponent
    {
        void SetActiveSolution([CanBeNull] string path);
    }
}
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.SourceControl
{
    public interface ISolutionStateTracker
    {
        void SetActiveSolution([CanBeNull] string path);
    }
}
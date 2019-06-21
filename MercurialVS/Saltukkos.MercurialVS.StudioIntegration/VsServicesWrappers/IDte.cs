using EnvDTE;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers
{
    public interface IDte
    {
        [CanBeNull] 
        Document ActiveDocument { get; }

        [CanBeNull]
        SelectedItems SelectedItems { get; }

        [CanBeNull]
        Solution Solution { get; }
    }
}
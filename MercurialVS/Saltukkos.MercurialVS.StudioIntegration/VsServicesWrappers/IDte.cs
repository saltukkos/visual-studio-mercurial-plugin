using EnvDTE;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers
{
    public interface IDte
    {
        [CanBeNull] 
        Document ActiveDocument { get; }
    }
}
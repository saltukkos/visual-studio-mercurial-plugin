using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    public interface IGlyphController : IPackageComponent
    {
        int GetSccGlyph([NotNull] string[] paths, [NotNull] VsStateIcon[] glyphs, [CanBeNull] uint[] sccStatus);
    }
}
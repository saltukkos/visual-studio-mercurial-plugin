using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell.Interop;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    public interface IGlyphController
    {
        int GetSccGlyph([NotNull] string[] paths, [NotNull] VsStateIcon[] glyphs, [CanBeNull] uint[] sccStatus);
    }
}
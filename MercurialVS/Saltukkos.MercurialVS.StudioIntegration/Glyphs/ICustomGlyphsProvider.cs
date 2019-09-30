using Microsoft.VisualStudio.Shell.Interop;

namespace Saltukkos.MercurialVS.StudioIntegration.Glyphs
{
    public interface ICustomGlyphsProvider
    {
        VsStateIcon GetGlyphIcon(Glyph glyph);
    }
}
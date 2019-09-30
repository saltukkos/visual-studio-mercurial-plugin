namespace Saltukkos.MercurialVS.StudioIntegration.Glyphs
{
    public interface ICustomGlyphsProviderInitializer
    {
        int GetCustomGlyphList(uint baseIndex, out uint pdwImageListHandle);
    }
}
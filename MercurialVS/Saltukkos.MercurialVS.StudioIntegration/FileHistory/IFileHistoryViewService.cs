using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    public interface IFileHistoryViewService
    {
        bool ShowHistoryFor([NotNull] string filePath);
    }
}
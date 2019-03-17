using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    public interface IOpenFileService
    {
        void OpenFileFromWorkingDirectory([NotNull] string fullPath);

        void OpenFileFromRevision([NotNull] string fullPath, Revision revision);

        void OpenFileDiff([NotNull] string fullPath, Revision baseRevision, Revision? changedRevision = null);
    }
}
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    public interface IFileHistoryProviderInternal : IFileHistoryProvider
    {
        void SetCurrentSourceControlClientProvider([CanBeNull] ISourceControlClient client);
    }
}
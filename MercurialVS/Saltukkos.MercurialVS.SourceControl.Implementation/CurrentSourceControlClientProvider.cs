using System;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [Component(typeof(SolutionUnderSourceControlScope))]
    public sealed class CurrentSourceControlClientProvider : IDisposable
    {
        [NotNull]
        private readonly IFileHistoryProviderInternal _fileHistoryProviderInternal;

        public CurrentSourceControlClientProvider(
            [NotNull] IFileHistoryProviderInternal fileHistoryProviderInternal,
            [NotNull] ISourceControlClient sourceControlClient)
        {
            ThrowIf.Null(fileHistoryProviderInternal, nameof(fileHistoryProviderInternal));
            ThrowIf.Null(sourceControlClient, nameof(sourceControlClient));
            _fileHistoryProviderInternal = fileHistoryProviderInternal;
            _fileHistoryProviderInternal.SetCurrentSourceControlClientProvider(sourceControlClient);
        }

        public void Dispose()
        {
            _fileHistoryProviderInternal.SetCurrentSourceControlClientProvider(null);
        }
    }
}
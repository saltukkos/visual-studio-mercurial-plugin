using System;
using System.IO;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [Component(typeof(PackageScope))]
    public sealed class FileHistoryProvider : IFileHistoryProviderInternal
    {
        private ISourceControlClient _client;

        public bool ExecuteWithFileAtCurrentRevision(string path, Action<string> action)
        {
            ThrowIf.Null(action, nameof(action));
            ThrowIf.Null(path, nameof(path));

            if (_client == null)
            {
                return false;
            }

            var fileAtCurrentRevision = _client.GetFileAtCurrentRevision(path);
            try
            {
                action(fileAtCurrentRevision);
            }
            finally
            {
                File.Delete(fileAtCurrentRevision);
            }

            return true;
        }

        public void SetCurrentSourceControlClientProvider(ISourceControlClient client)
        {
            _client = client;
        }
    }
}
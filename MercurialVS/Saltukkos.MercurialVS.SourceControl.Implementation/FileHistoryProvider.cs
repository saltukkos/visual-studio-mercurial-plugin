﻿using System;
using System.Collections.Generic;
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

        public bool ExecuteWithFileAtRevision(string path, Revision? revision, Action<string> action)
        {
            ThrowIf.Null(action, nameof(action));
            ThrowIf.Null(path, nameof(path));

            if (_client == null)
            {
                return false;
            }

            if (revision == null)
            {
                action(path);
                return true;
            }

            var fileAtCurrentRevision = _client.GetFileAtRevision(path, revision.Value);
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

        public IReadOnlyList<ChangeSet> GetFileChangesHistory(string path)
        {
            ThrowIf.Null(path, nameof(path));
            return _client?.GetFileHistoryLog(path);
        }

        public void SetCurrentSourceControlClientProvider(ISourceControlClient client)
        {
            _client = client;
        }
    }
}
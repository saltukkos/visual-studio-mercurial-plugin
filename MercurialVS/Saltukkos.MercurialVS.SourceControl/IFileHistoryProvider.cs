using System;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.SourceControl
{
    public interface IFileHistoryProvider
    {
        bool ExecuteWithFileAtRevision([NotNull] string path, Revision? revision, [NotNull] Action<string> action);
    }
}
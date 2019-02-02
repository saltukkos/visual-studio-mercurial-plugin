using System;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.SourceControl
{
    public interface IFileHistoryProvider
    {
        bool ExecuteWithFileAtCurrentRevision([NotNull] string path, [NotNull] Action<string> action);
    }
}
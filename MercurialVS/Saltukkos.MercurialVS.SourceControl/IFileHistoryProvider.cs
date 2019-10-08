using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.SourceControl
{
    public interface IFileHistoryProvider
    {
        bool ExecuteWithFileAtRevision([NotNull] string path, Revision? revision, [NotNull] Action<string> action);

        [CanBeNull]
        IReadOnlyList<ChangeSet> GetFileChangesHistory([NotNull] string path);

        [CanBeNull]
        IReadOnlyList<DiffLine> GetDiffToParent([NotNull] string path, Revision revision);

        [CanBeNull]
        IReadOnlyList<AnnotationLine> AnnotateAtRevision([NotNull] string filename, Revision revision);
    }
}
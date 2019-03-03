using JetBrains.Annotations;
using Mercurial;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.HgServices.Implementation
{
    internal static class ChangeSetExtensions
    {
        public static ChangeSet ToChangeSet([NotNull] this Changeset changeset)
        {
            ThrowIf.Null(changeset, nameof(changeset));
            ThrowIf.Null(changeset.CommitMessage, nameof(changeset.CommitMessage));
            ThrowIf.Null(changeset.Branch, nameof(changeset.Branch));
            ThrowIf.Null(changeset.AuthorName, nameof(changeset.AuthorName));
            ThrowIf.Null(changeset.Hash, nameof(changeset.Hash));

            return new ChangeSet
            {
                CommitMessage = changeset.CommitMessage,
                Branch = changeset.Branch,
                Author = changeset.AuthorName,
                RevisionNumber = changeset.RevisionNumber,
                RevisionHash = changeset.Hash,
            };
        }
    }
}
using Mercurial;

namespace Saltukkos.MercurialVS.HgServices.Implementation
{
    internal static class RevisionExtensions
    {
        public static RevSpec ToRevSpec(this Revision revision)
        {
            return new RevSpec(revision.RevSpec);
        }
    }
}
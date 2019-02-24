using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.HgServices
{
    public struct Revision
    {
        //TODO is it mercurial.net bug that just "." not works
        public static Revision Current { get; } = new Revision(".^0");

        public Revision([NotNull] string revSpec)
        {
            ThrowIf.Null(revSpec, nameof(revSpec));
            RevSpec = revSpec;
        }

        [NotNull] public string RevSpec { get; }
    }
}
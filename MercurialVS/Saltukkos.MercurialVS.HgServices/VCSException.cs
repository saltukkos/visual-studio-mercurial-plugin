using System;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.HgServices
{
    public class VCSException : Exception
    {
        public VCSException([CanBeNull] string message = null) : base(message)
        {
        }
    }
}
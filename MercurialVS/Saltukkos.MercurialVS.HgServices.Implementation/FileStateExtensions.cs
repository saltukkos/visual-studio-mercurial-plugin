using System;

namespace Saltukkos.MercurialVS.HgServices.Implementation
{
    internal static class FileStateExtensions
    {
        public static FileStatus ToFileStatus(this Mercurial.FileState mercurialState)
        {
            switch (mercurialState)
            {
                case Mercurial.FileState.Unknown:
                    return FileStatus.Unknown;
                case Mercurial.FileState.Modified:
                    return FileStatus.Modified;
                case Mercurial.FileState.Added:
                    return FileStatus.Added;
                case Mercurial.FileState.Removed:
                    return FileStatus.Removed;
                case Mercurial.FileState.Clean:
                    return FileStatus.Clean;
                case Mercurial.FileState.Missing:
                    return FileStatus.Missing;
                case Mercurial.FileState.Ignored:
                    return FileStatus.Ignored;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mercurialState), mercurialState, null);
            }
        }
    }
}
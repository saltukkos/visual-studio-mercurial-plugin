using System;
using System.Diagnostics;
using System.IO;
using Mercurial;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.HgServices.Implementation
{
    [Component(typeof(PackageScope))]
    public class SourceControlBasePathProvider : ISourceControlBasePathProvider
    {
        public SourceControlBasePathProvider()
        {
            if (!Client.CouldLocateClient)
            {
                throw new NotSupportedException("Could not locate hg client");
            }

            Trace.WriteLine($"Found hg client, path: {Client.ClientPath}");
        }

        public bool TryGetBasePath(string solutionPath, out string basePath)
        {
            ThrowIf.Null(solutionPath, nameof(solutionPath));
            basePath = null;

            try
            {
                var directory = Directory.GetParent(solutionPath);
                while (directory != null)
                {
                    if (directory.GetDirectories(".hg").Length == 1)
                    {
                        basePath = directory.FullName;
                        return true;
                    }

                    directory = directory.Parent;
                }

                return false;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
        }
    }
}
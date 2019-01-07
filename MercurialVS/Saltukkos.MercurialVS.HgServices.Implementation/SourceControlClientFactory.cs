using System;
using System.Diagnostics;
using System.IO;
using Mercurial;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.HgServices.Implementation
{
    [PackageComponent]
    public class SourceControlClientFactory : ISourceControlClientFactory
    {
        public SourceControlClientFactory()
        {
            if (!Client.CouldLocateClient)
            {
                throw new NotSupportedException("Could not locate hg client");
            }

            Trace.WriteLine($"Found hg client, path: {Client.ClientPath}");
        }

        public bool TryCreateClient(string solutionPath, out ISourceControlClient client)
        {
            client = null;
            try
            {
                var directory = Directory.GetParent(solutionPath);
                while (directory != null)
                {
                    if (directory.GetDirectories(".hg").Length == 1)
                    {
                        client = new SourceControlClient(directory.FullName);
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
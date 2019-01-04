using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mercurial;

namespace Saltukkos.MercurialVS.HgServices.Implementation
{
    public sealed class SourceControlClient : ISourceControlClient
    {
        public SourceControlClient([NotNull] string rootPath)
        {
            RootPath = rootPath;
        }

        public string RootPath { get; }

        public IReadOnlyList<FileState> GetNotCleanFiles()
        {
            return GetFilesStatesInternal(FileStatusIncludes.Default);
        }

        public IReadOnlyList<FileState> GetAllFilesStates()
        {
            return GetFilesStatesInternal(FileStatusIncludes.All);
        }

        [NotNull]
        private IReadOnlyList<FileState> GetFilesStatesInternal(FileStatusIncludes includeStates)
        {
            var statusCommand = new StatusCommand {Include = includeStates};
            Client.Execute(RootPath, statusCommand);

            return (statusCommand.Result ?? throw new InvalidOperationException("Command result is null"))
                .Where(x => x?.Path != null)
                .Select(x => new FileState(x.Path, ToFileStatus(x.State))).ToList();
        }

        private FileStatus ToFileStatus(Mercurial.FileState mercurialState)
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
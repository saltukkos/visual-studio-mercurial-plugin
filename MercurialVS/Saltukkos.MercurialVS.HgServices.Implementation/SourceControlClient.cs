using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Mercurial;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.HgServices.Implementation
{
    [Component(typeof(SolutionUnderSourceControlScope))]
    public sealed class SourceControlClient : ISourceControlClient
    {
        [NotNull]
        private readonly string _rootPath;

        public SourceControlClient([NotNull] SolutionUnderSourceControlInfo solutionUnderSourceControlInfo)
        {
            _rootPath = solutionUnderSourceControlInfo.SourceControlDirectoryPath;
        }

        public string GetFileAtCurrentRevision(string filename)
        {
            ThrowIf.Null(filename, nameof(filename));
            var fileName = $"{Guid.NewGuid():N}-{Path.GetFileName(filename)}";
            var tempFile = Path.Combine(Path.GetTempPath(), fileName);
            var catCommand = new CatCommand {OutputFormat = tempFile}.WithFile(filename);
            Client.Execute(_rootPath, catCommand);
            return tempFile;
        }

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
            Client.Execute(_rootPath, statusCommand);

            return (statusCommand.Result ?? throw new InvalidOperationException("Command result is null"))
                .Where(x => x?.Path != null)
                .Select(x => new FileState(
                    Path.GetFullPath($"{_rootPath}{Path.DirectorySeparatorChar}{x.Path}"),
                    ToFileStatus(x.State)))
                .ToList();
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
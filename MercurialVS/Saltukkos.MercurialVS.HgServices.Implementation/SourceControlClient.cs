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
    internal sealed class SourceControlClient : ISourceControlClient
    {
        [NotNull]
        private readonly string _rootPath;

        [NotNull]
        private readonly IClient _client;

        public SourceControlClient([NotNull] SolutionUnderSourceControlInfo solutionUnderSourceControlInfo)
        {
            ThrowIf.Null(solutionUnderSourceControlInfo, nameof(solutionUnderSourceControlInfo));
            _rootPath = solutionUnderSourceControlInfo.SourceControlDirectoryPath;
            _client = new NonPersistentClient(_rootPath);
        }

        public IReadOnlyList<ChangeSet> GetFileHistoryLog(string filename)
        {
            ThrowIf.Null(filename, nameof(filename));
            var logCommand = new LogCommand()
                .WithPath(filename);
            _client.Execute(logCommand);
            var logCommandResult = logCommand.Result;
            ThrowIf.Null(logCommandResult, nameof(logCommandResult));
            return logCommandResult.Select(c => c.ToChangeSet()).ToList();
        }

        public string GetFileAtRevision(string filename, Revision revision)
        {
            ThrowIf.Null(filename, nameof(filename));
            var fileName = $"{Guid.NewGuid():N}-{Path.GetFileName(filename)}";
            var tempFile = Path.Combine(Path.GetTempPath(), fileName);
            var catCommand = new CatCommand()
                .WithFile(filename)
                .WithOutputFormat(tempFile)
                .WithRevision(revision.ToRevSpec());
            _client.Execute(catCommand);
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
            _client.Execute(statusCommand);

            return (statusCommand.Result ?? throw new InvalidOperationException("Command result is null"))
                .Where(x => x?.Path != null)
                .Select(x => new FileState(
                    Path.GetFullPath($"{_rootPath}{Path.DirectorySeparatorChar}{x.Path}"),
                    x.State.ToFileStatus(),
                    x.Path))
                .ToList();
        }
    }
}
﻿using System;
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
            ExecuteCommand(logCommand);
            var logCommandResult = logCommand.Result;
            ThrowIf.Null(logCommandResult, nameof(logCommandResult));
            return logCommandResult.Select(c => c.ToChangeSet()).ToList();
        }

        public IReadOnlyList<AnnotationLine> AnnotateAtRevision(string filename, Revision revision)
        {
            ThrowIf.Null(filename, nameof(filename));
            var command = new AnnotateCommand()
            {
                Path = filename,
                Revision = revision.ToRevSpec()
            };
            ExecuteCommand(command);
            var result = command.Result;
            ThrowIf.Null(result, nameof(result));
            return result.Select(annotation => new AnnotationLine(
                    // ReSharper disable once AssignNullToNotNullAttribute <-- Can't process Select on [ItemNotNul]
                    line: annotation.Line,
                    lineNumber: annotation.LineNumber,
                    revision: annotation.RevisionNumber))
                .ToList();
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
            ExecuteCommand(catCommand);
            return tempFile;
        }

        public IReadOnlyList<DiffLine> GetDiffToParent(string filename, Revision revision)
        {
            var command = new DiffCommand
            {
                ChangeIntroducedByRevision = revision.ToRevSpec(),
                Names = {filename}
            };
            ExecuteCommand(command);
            if (command.Result is null)
            {
                throw new VCSException("Empty result");
            }

            return ParseDiffLines(command.Result);
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
            ExecuteCommand(statusCommand);

            return (statusCommand.Result ?? throw new InvalidOperationException("Command result is null"))
                .Where(x => x?.Path != null)
                .Select(x => new FileState(
                    Path.GetFullPath($"{_rootPath}{Path.DirectorySeparatorChar}{x.Path}"),
                    x.State.ToFileStatus(),
                    x.Path))
                .ToList();
        }

        private void ExecuteCommand([NotNull] IMercurialCommand logCommand)
        {
            try
            {
                _client.Execute(logCommand);
            }
            catch (MercurialException exception)
            {
                throw new VCSException(exception.Message);
            }
        }

        [NotNull]
        private IReadOnlyList<DiffLine> ParseDiffLines([NotNull] string diff)
        {
            return diff.Split('\r', '\n')
                .Skip(3) //skip diff description
                // ReSharper disable once PossibleNullReferenceException <-- loose nullability on Skip()
                .Where(l => l.Length > 0)
                .Select(l => new DiffLine(l, GetDiffLineType(l[0])))
                .ToList();
        }

        private static DiffLineType GetDiffLineType(char firstChar)
        {
            DiffLineType diffLineType;
            switch (firstChar)
            {
                case '+':
                    diffLineType = DiffLineType.AddLine;
                    break;
                case '-':
                    diffLineType = DiffLineType.RemoveLine;
                    break;
                case '@':
                    diffLineType = DiffLineType.MetaInfoLine;
                    break;
                default:
                    diffLineType = DiffLineType.ContextLine;
                    break;
            }

            return diffLineType;
        }
    }
}
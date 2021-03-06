﻿using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [Component(typeof(SolutionUnderSourceControlScope))]
    public sealed class SolutionStateTracker : IDisposable
    {
        private const int RefreshPendingInMilliseconds = 200;

        [NotNull]
        [ItemNotNull]
        private static readonly string[] IgnoredPaths = {".hg", ".vs"};

        [NotNull]
        [ItemNotNull]
        private static readonly string[] ExclusionsFromIgnoredPaths = {"dirstate"};

        [NotNull]
        private readonly IDirectoryWatcherWithPending _directoryWatcherWithPending;

        [NotNull]
        private readonly IDirectoryStateProviderInternal _directoryStateProvider;

        [NotNull]
        private readonly ISourceControlClient _currentSourceControlClient;

        public SolutionStateTracker(
            [NotNull] SolutionUnderSourceControlInfo solutionUnderSourceControlInfo,
            [NotNull] IDirectoryWatcherWithPending directoryWatcherWithPending,
            [NotNull] IDirectoryStateProviderInternal directoryStateProvider,
            [NotNull] ISourceControlClient currentSourceControlClient)
        {
            ThrowIf.Null(solutionUnderSourceControlInfo, nameof(solutionUnderSourceControlInfo));
            ThrowIf.Null(directoryWatcherWithPending, nameof(directoryWatcherWithPending));
            ThrowIf.Null(directoryStateProvider, nameof(directoryStateProvider));
            ThrowIf.Null(currentSourceControlClient, nameof(currentSourceControlClient));

            _currentSourceControlClient = currentSourceControlClient;
            _directoryStateProvider = directoryStateProvider;

            _directoryWatcherWithPending = directoryWatcherWithPending;
            _directoryWatcherWithPending.PendingInMilliseconds = RefreshPendingInMilliseconds;
            _directoryWatcherWithPending.Path = solutionUnderSourceControlInfo.SourceControlDirectoryPath;
            _directoryWatcherWithPending.OnDirectoryChanged += OnDirectoryChanged;
            _directoryWatcherWithPending.IncludeFilter = path =>
            {
                ThrowIf.Null(path, nameof(path));
                var pathFragments = path.Split(Path.DirectorySeparatorChar);
                return ExclusionsFromIgnoredPaths.Any(pathFragments.Contains) ||
                       !IgnoredPaths.Any(ignored => pathFragments.Contains(ignored));
            };
            _directoryWatcherWithPending.RaiseEvents = true;

            OnDirectoryChanged();
        }

        public void Dispose()
        {
            _directoryStateProvider.SetNewDirectoryStatus(new FileState[0]);
            _directoryWatcherWithPending.RaiseEvents = false;
            _directoryWatcherWithPending.OnDirectoryChanged -= OnDirectoryChanged;
        }

        private void OnDirectoryChanged()
        {
            var allFilesStates = _currentSourceControlClient.GetAllFilesStates();
            _directoryStateProvider.SetNewDirectoryStatus(allFilesStates);
        }

        private void OnDirectoryChanged(object sender, EventArgs e)
        {
            OnDirectoryChanged();
        }
    }
}
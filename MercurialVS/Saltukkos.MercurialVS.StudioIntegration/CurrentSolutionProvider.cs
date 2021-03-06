﻿using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Component(typeof(SourceControlScope))]
    public class CurrentSolutionProvider : IDisposable, IVsSolutionEvents
    {
        private readonly uint _solutionEventsSubscriberId;

        private bool _sourceControlScopeStarted;

        [NotNull]
        private readonly IVsSolution _vsSolution;

        [NotNull]
        private readonly ILifetimeScopeManager<SolutionUnderSourceControlScope, SolutionUnderSourceControlInfo>
            _solutionLifetimeScopeManager;

        [NotNull]
        private readonly ISourceControlBasePathProvider _sourceControlBasePathProvider;

        public CurrentSolutionProvider(
            [NotNull] IVsSolution vsSolution,
            [NotNull] ILifetimeScopeManager<SolutionUnderSourceControlScope, SolutionUnderSourceControlInfo>
                solutionLifetimeScopeManager,
            [NotNull] ISourceControlBasePathProvider sourceControlBasePathProvider)
        {
            ThrowIf.Null(vsSolution, nameof(vsSolution));
            ThrowIf.Null(solutionLifetimeScopeManager, nameof(solutionLifetimeScopeManager));
            ThrowIf.Null(sourceControlBasePathProvider, nameof(sourceControlBasePathProvider));

            _vsSolution = vsSolution;
            _solutionLifetimeScopeManager = solutionLifetimeScopeManager;
            _sourceControlBasePathProvider = sourceControlBasePathProvider;

            _vsSolution.AdviseSolutionEvents(this, out _solutionEventsSubscriberId);
            TryStartSolutionLifetimeScope();
        }

        public void Dispose()
        {
            _vsSolution.UnadviseSolutionEvents(_solutionEventsSubscriberId);
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            TryStartSolutionLifetimeScope();
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            TryCloseSolutionLifetimeScope();
            return VSConstants.S_OK;
        }

        private void TryStartSolutionLifetimeScope()
        {
            if (_sourceControlScopeStarted)
            {
                Debugger.Break();
                _solutionLifetimeScopeManager.EndScopeLifetime();
            }

            if (_vsSolution.GetSolutionInfo(out var solutionDirectory, out _, out _) != VSConstants.S_OK ||
                solutionDirectory is null)
            {
                return;
            }

            if (!_sourceControlBasePathProvider.TryGetBasePath(solutionDirectory, out var sourceControlBasePath))
            {
                return;
            }

            var solutionInfo = new SolutionUnderSourceControlInfo(
                solutionDirectoryPath: solutionDirectory,
                sourceControlDirectoryPath: sourceControlBasePath);

            _solutionLifetimeScopeManager.StartScopeLifetime(solutionInfo);
            _sourceControlScopeStarted = true;
        }

        private void TryCloseSolutionLifetimeScope()
        {
            if (!_sourceControlScopeStarted)
            {
                Debugger.Break();
                return;
            }

            _solutionLifetimeScopeManager.EndScopeLifetime();
            _sourceControlScopeStarted = false;
        }

        #region #region Unused methods
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }
        #endregion
    }
}
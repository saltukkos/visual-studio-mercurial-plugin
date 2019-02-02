using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.MercurialVS.SourceControl;
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
        private readonly ILifetimeScopeManager<SolutionUnderSourceControlScope> _solutionLifetimeScopeManager;

        [NotNull]
        private readonly ISourceControlBasePathProvider _sourceControlBasePathProvider;

        public CurrentSolutionProvider(
            [NotNull] IVsSolution vsSolution,
            [NotNull] ILifetimeScopeManager<SolutionUnderSourceControlScope> solutionLifetimeScopeManager,
            [NotNull] ISourceControlBasePathProvider sourceControlBasePathProvider)
        {
            ThrowIf.Null(vsSolution, nameof(vsSolution));
            ThrowIf.Null(solutionLifetimeScopeManager, nameof(solutionLifetimeScopeManager));
            ThrowIf.Null(sourceControlBasePathProvider, nameof(sourceControlBasePathProvider));

            _vsSolution = vsSolution;
            _solutionLifetimeScopeManager = solutionLifetimeScopeManager;
            _sourceControlBasePathProvider = sourceControlBasePathProvider;

            _vsSolution.AdviseSolutionEvents(this, out _solutionEventsSubscriberId);
        }

        public void Dispose()
        {
            _vsSolution.UnadviseSolutionEvents(_solutionEventsSubscriberId);
        }

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

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            if (_sourceControlScopeStarted)
            {
                Debugger.Break();
                _solutionLifetimeScopeManager.EndScopeLifetime();
            }

            if (_vsSolution.GetSolutionInfo(out var solutionDirectory, out _, out _) != VSConstants.S_OK)
            {
                return VSConstants.S_OK;
            }

            if (!_sourceControlBasePathProvider.TryGetBasePath(solutionDirectory, out var sourceControlBasePath))
            {
                return VSConstants.S_OK;
            }

            var solutionInfo = new SolutionUnderSourceControlInfo(
                solutionDirectoryPath: solutionDirectory,
                sourceControlDirectoryPath: sourceControlBasePath);

            _solutionLifetimeScopeManager.StartScopeLifetime(solutionInfo);
            _sourceControlScopeStarted = true;
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            if (_sourceControlScopeStarted)
            {
                _solutionLifetimeScopeManager.EndScopeLifetime();
            }

            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }
    }
}
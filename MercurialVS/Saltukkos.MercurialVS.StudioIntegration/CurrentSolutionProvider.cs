using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.SourceControl;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [SourceControlComponent]
    public class CurrentSolutionProvider : IDisposable, ISourceControlComponent, IVsSolutionEvents
    {
        private readonly uint _solutionEventsSubscriberId;

        [NotNull]
        private readonly ISolutionStateTracker _solutionStateTracker;

        [NotNull]
        private readonly IVsSolution _vsSolution;

        public CurrentSolutionProvider(
            [NotNull] ISolutionStateTracker solutionStateTracker,
            [NotNull] IVsSolution vsSolution)
        {
            _solutionStateTracker = solutionStateTracker;
            _vsSolution = vsSolution;

            NotifyActiveSolution();

            _vsSolution.AdviseSolutionEvents(this, out _solutionEventsSubscriberId);
        }

        private void NotifyActiveSolution()
        {
            if (_vsSolution.GetSolutionInfo(out var solutionDirectory, out _, out _) != VSConstants.S_OK)
            {
                return;
            }

            _solutionStateTracker.SetActiveSolution(solutionDirectory);
        }

        public void Dispose()
        {
            _vsSolution.UnadviseSolutionEvents(_solutionEventsSubscriberId);
            _solutionStateTracker.SetActiveSolution(null);
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
            NotifyActiveSolution();
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            _solutionStateTracker.SetActiveSolution(null);
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }
    }
}
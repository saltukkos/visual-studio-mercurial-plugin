using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Component(typeof(PackageScope))]
    public sealed class VsIdleNotifier : IDisposable, IOleComponent, IVsIdleNotifier
    {
        [NotNull] private readonly IOleComponentManager _oleComponentManager;

        private readonly uint _componentId;

        public event Action IdlingStarted;

        public VsIdleNotifier([NotNull] IOleComponentManager oleComponentManager)
        {
            ThrowIf.Null(oleComponentManager, nameof(oleComponentManager));
            _oleComponentManager = oleComponentManager;

            var pcrInfo = new OLECRINFO[1];
            pcrInfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
            pcrInfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime |
                                (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
            pcrInfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal |
                                  (uint)_OLECADVF.olecadvfRedrawOff |
                                  (uint)_OLECADVF.olecadvfWarningsOff;
            pcrInfo[0].uIdleTimeInterval = 200;

            _oleComponentManager.FRegisterComponent(this, pcrInfo, out _componentId);
        }

        public void Dispose()
        {
            _oleComponentManager.FRevokeComponent(_componentId);
        }

        public int FDoIdle(uint grfidlef)
        {
            IdlingStarted?.Invoke();
            return VSConstants.S_OK;
        }

        #region Unused methods

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 0;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
            return 0;
        }

        public void OnEnterState(uint uStateId, int fEnter)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadId)
        {
        }

        public void OnLoseActivation()
        {
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating,
            OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public void Terminate()
        {
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }
        
        #endregion
    }
}
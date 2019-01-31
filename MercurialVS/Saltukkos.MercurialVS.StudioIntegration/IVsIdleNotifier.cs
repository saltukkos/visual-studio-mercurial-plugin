using System;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    public interface IVsIdleNotifier
    {
        event Action IdlingStarted;
    }
}
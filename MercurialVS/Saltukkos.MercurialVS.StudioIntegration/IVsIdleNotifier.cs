using System;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    public interface IVsIdleNotifier : IPackageComponent
    {
        event Action IdlingStarted;
    }
}
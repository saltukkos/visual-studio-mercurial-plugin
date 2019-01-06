using System;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    public interface IDirectoryWatcherWithPending : IPackageComponent
    {
        [NotNull]
        string Path { get; set; }

        int PendingInMilliseconds { get; set; }

        [CanBeNull]
        Predicate<string> IncludeFilter { get; set; }

        bool RaiseEvents { get; set; }

        event EventHandler OnDirectoryChanged;
    }
}
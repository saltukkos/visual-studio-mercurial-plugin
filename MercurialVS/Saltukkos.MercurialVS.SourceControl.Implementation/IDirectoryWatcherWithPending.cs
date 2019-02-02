using System;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    //TODO it's not actually SingleInstance, it's more like per-request instance
    public interface IDirectoryWatcherWithPending
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
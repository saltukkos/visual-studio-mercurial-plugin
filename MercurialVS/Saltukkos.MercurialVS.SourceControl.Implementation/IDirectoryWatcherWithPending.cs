using System;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
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
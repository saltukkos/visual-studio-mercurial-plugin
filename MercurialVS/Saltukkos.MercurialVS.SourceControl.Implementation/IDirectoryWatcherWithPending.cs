using System;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    public interface IDirectoryWatcherWithPending : IDisposable
    {
        [NotNull]
        string Path { get; set; }

        bool RaiseEvents { get; set; }

        event EventHandler OnDirectoryChanged;
    }
}
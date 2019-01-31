using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.SourceControl
{
    public interface IDirectoryStateProvider
    {
        //TODO filter events for different flags combination to prevent redundant updates
        event EventHandler DirectoryStateChanged;

        [NotNull]
        IReadOnlyList<FileState> CurrentStatus { get; }
    }
}
using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;

namespace Saltukkos.MercurialVS.Package
{
    public interface IToolWindowContainer
    {
        [CanBeNull]
        ToolWindowPane FindToolWindow([NotNull] Type toolWindowType, int id, bool create);
    }
}
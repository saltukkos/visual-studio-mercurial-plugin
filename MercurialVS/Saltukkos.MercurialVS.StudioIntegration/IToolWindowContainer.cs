using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    public interface IToolWindowContainer
    {
        [CanBeNull]
        ToolWindowPane FindToolWindow([NotNull] Type toolWindowType, int id, bool create);
    }
}
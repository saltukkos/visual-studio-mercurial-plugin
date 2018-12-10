using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;

namespace StudioIntegrationPackage
{
    public interface IToolWindowContainer
    {
        [CanBeNull]
        ToolWindowPane FindToolWindow([NotNull] Type toolWindowType, int id, bool create);
    }
}
using System.Collections.Generic;
using System.ComponentModel.Design;
using JetBrains.Annotations;

namespace StudioIntegrationPackage.Commands
{
    public interface ICommandsRegistry
    {
        [NotNull]
        [ItemNotNull]
        IReadOnlyCollection<MenuCommand> RegisteredCommands { get; }
    }
}
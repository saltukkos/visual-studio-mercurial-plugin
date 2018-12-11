using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    [Component]
    public class CommandsRegistry : ICommandsRegistry, IPackageComponent
    {
        public IReadOnlyCollection<MenuCommand> RegisteredCommands { get; }

        public CommandsRegistry(
            [NotNull] IMenuCommandService menuCommandService,
            [ItemNotNull] [NotNull] IReadOnlyCollection<ICommand> commands)
        {
            var menuCommands = new List<MenuCommand>(commands.Count);
            foreach (var command in commands)
            {
                var commandId = new CommandID(Guid.Parse(Constants.CommandSetGuid), command.CommandId);
                var menuCommand = new MenuCommand((sender, args) => command.Invoke(), commandId);
                menuCommandService.AddCommand(menuCommand);
                menuCommands.Add(menuCommand);
            }

            RegisteredCommands = menuCommands;
        }
    }
}
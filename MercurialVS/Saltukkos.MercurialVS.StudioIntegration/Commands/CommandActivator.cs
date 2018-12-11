using System;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    [Component]
    public class CommandActivator : ISourceControlComponent, IDisposable
    {
        [NotNull]
        private readonly ICommandsRegistry _commandsRegistry;

        public CommandActivator([NotNull] ICommandsRegistry commandsRegistry)
        {
            _commandsRegistry = commandsRegistry;

            foreach (var menuCommand in commandsRegistry.RegisteredCommands)
            {
                menuCommand.Enabled = true;
                menuCommand.Visible = true;
            }
        }

        public void Dispose()
        {
            foreach (var menuCommand in _commandsRegistry.RegisteredCommands)
            {
                menuCommand.Enabled = false;
                menuCommand.Visible = false;
            }
        }
    }
}
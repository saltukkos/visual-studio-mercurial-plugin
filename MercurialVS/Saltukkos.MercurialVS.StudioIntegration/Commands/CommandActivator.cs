using System;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    [Component(typeof(SourceControlScope))]
    public class CommandActivator : IDisposable
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
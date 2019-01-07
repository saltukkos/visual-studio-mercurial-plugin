using System.Collections.Generic;
using System.ComponentModel.Design;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    public interface ICommandsRegistry : IPackageComponent
    {
        [NotNull]
        [ItemNotNull]
        IReadOnlyCollection<MenuCommand> RegisteredCommands { get; }
    }
}
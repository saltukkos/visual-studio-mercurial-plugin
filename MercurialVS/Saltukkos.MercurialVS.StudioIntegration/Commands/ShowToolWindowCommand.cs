﻿using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    [Component(typeof(PackageScope))]
    public class ShowToolWindowCommand : ICommand
    {
        [NotNull]
        private readonly IToolWindowContainer _toolWindowContainer;

        public ShowToolWindowCommand([NotNull] IToolWindowContainer toolWindowContainer)
        {
            ThrowIf.Null(toolWindowContainer, nameof(toolWindowContainer));
            _toolWindowContainer = toolWindowContainer;
        }

        public int CommandId => Constants.ShowToolWindowCommandId;

        public void Invoke()
        {
            var toolWindow = _toolWindowContainer.FindToolWindow(typeof(MainToolWindow), 0, true);
            ((IVsWindowFrame) toolWindow?.Frame)?.Show();
        }
    }
}
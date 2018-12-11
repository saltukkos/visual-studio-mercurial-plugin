using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    [Component]
    public class ShowToolWindowCommand : ICommand
    {
        [NotNull]
        private readonly IToolWindowContainer _toolWindowContainer;

        public ShowToolWindowCommand([NotNull] IToolWindowContainer toolWindowContainer)
        {
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
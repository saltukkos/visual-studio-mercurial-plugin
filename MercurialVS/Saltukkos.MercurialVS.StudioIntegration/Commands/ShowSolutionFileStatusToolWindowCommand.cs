using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.StudioIntegration.SolutionFilesStatus;
using Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    [Component(typeof(PackageScope))]
    public class ShowSolutionFileStatusToolWindowCommand : ICommand
    {
        [NotNull]
        private readonly IToolWindowContainer _toolWindowContainer;

        public ShowSolutionFileStatusToolWindowCommand([NotNull] IToolWindowContainer toolWindowContainer)
        {
            ThrowIf.Null(toolWindowContainer, nameof(toolWindowContainer));
            _toolWindowContainer = toolWindowContainer;
        }

        public int CommandId => Constants.ShowSolutionFileStatusToolWindowCommandId;

        public void Invoke()
        {
            var toolWindow = _toolWindowContainer.FindToolWindow(typeof(SolutionFilesStatusToolWindow), 0, true);
            toolWindow?.Show();
        }
    }
}
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.StudioIntegration.FileHistory;
using Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    [Component(typeof(PackageScope))]
    public class ShowSolutionFileHistoryCommand : ICommand
    {
        [NotNull]
        private readonly IDte _dte;

        [NotNull]
        private readonly IFileHistoryViewService _fileHistoryViewService;

        public ShowSolutionFileHistoryCommand(
            [NotNull] IDte dte,
            [NotNull] IFileHistoryViewService fileHistoryViewService)
        {
            ThrowIf.Null(fileHistoryViewService, nameof(fileHistoryViewService));
            ThrowIf.Null(dte, nameof(dte));
            _dte = dte;
            _fileHistoryViewService = fileHistoryViewService;
        }

        public int CommandId => Constants.ShowSolutionFileLogCommandId;

        public void Invoke()
        {
            var solution = _dte.Solution;
            var fileName = solution?.FullName;

            if (fileName != null)
            {
                _fileHistoryViewService.ShowHistoryFor(Ensure.NotNull(fileName));
            }
        }
    }
}
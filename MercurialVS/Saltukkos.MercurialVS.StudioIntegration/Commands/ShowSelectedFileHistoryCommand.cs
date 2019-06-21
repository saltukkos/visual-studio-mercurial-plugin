using EnvDTE;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.StudioIntegration.FileHistory;
using Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    //TODO make solution-under-source-control-scoped
    [Component(typeof(PackageScope))]
    public class ShowSelectedFileHistoryCommand : ICommand
    {
        [NotNull]
        private readonly IDte _dte;

        [NotNull]
        private readonly IFileHistoryViewService _fileHistoryViewService;

        public ShowSelectedFileHistoryCommand(
            [NotNull] IDte dte,
            [NotNull] IFileHistoryViewService fileHistoryViewService)
        {
            ThrowIf.Null(fileHistoryViewService, nameof(fileHistoryViewService));
            ThrowIf.Null(dte, nameof(dte));
            _dte = dte;
            _fileHistoryViewService = fileHistoryViewService;
        }

        public int CommandId => Constants.ShowSelectedFileLogCommandId;

        public void Invoke()
        {
            var selectedItems = _dte.SelectedItems;
            if (selectedItems is null)
            {
                return;
            }

            foreach (SelectedItem selectedItem in selectedItems)
            {
                string fileName = null;
                if (selectedItem.ProjectItem != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    fileName = selectedItem.ProjectItem.FileNames[0];
                }
                else if (selectedItem.Project != null)
                {
                    fileName = selectedItem.Project.FileName;
                }

                if (fileName != null)
                {
                    _fileHistoryViewService.ShowHistoryFor(Ensure.NotNull(fileName));
                }
            }
        }
    }

    //TODO make solution-under-source-control-scoped
}
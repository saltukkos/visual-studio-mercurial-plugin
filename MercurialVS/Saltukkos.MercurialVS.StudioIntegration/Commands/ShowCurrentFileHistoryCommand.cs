﻿using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.StudioIntegration.FileHistory;
using Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.Commands
{
    //TODO make solution-under-source-control-scoped
    [Component(typeof(PackageScope))]
    public class ShowCurrentFileHistoryCommand : ICommand
    {
        [NotNull]
        private readonly IDte _dte;

        [NotNull]
        private readonly IFileHistoryViewService _fileHistoryViewService;

        public ShowCurrentFileHistoryCommand([NotNull] IDte dte, [NotNull] IFileHistoryViewService fileHistoryViewService)
        {
            ThrowIf.Null(fileHistoryViewService, nameof(fileHistoryViewService));
            ThrowIf.Null(dte, nameof(dte));
            _dte = dte;
            _fileHistoryViewService = fileHistoryViewService;
        }

        public int CommandId => Constants.ShowCurrentFileLogCommandId;

        public void Invoke()
        {
            var document = _dte.ActiveDocument;
            if (document is null)
            {
                return;
            }

            var documentFullName = document.FullName;
            _fileHistoryViewService.ShowHistoryFor(Ensure.NotNull(documentFullName));
        }
    }
}
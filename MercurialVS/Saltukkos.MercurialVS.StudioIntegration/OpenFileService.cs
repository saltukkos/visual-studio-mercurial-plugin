using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.MercurialVS.SourceControl;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Component(typeof(PackageScope))]
    internal sealed class OpenFileService : IOpenFileService
    {
        [NotNull]
        private readonly IVsUIShellOpenDocument _uiShellOpenDocument;

        [NotNull]
        private readonly IFileHistoryProvider _fileHistoryProvider;

        [NotNull]
        private readonly IVsDifferenceService _vsDifferenceService;

        public OpenFileService(
            [NotNull] IVsUIShellOpenDocument uiShellOpenDocument,
            [NotNull] IFileHistoryProvider fileHistoryProvider,
            [NotNull] IVsDifferenceService vsDifferenceService)
        {
            ThrowIf.Null(vsDifferenceService, nameof(vsDifferenceService));
            ThrowIf.Null(fileHistoryProvider, nameof(fileHistoryProvider));
            ThrowIf.Null(uiShellOpenDocument, nameof(uiShellOpenDocument));
            _uiShellOpenDocument = uiShellOpenDocument;
            _fileHistoryProvider = fileHistoryProvider;
            _vsDifferenceService = vsDifferenceService;
        }

        public void OpenFileFromWorkingDirectory(string fullPath)
        {
            ThrowIf.Null(fullPath, nameof(fullPath));
            OpenFileInEditor(fullPath);
        }

        public void OpenFileFromRevision(string fullPath, Revision revision)
        {
            ThrowIf.Null(fullPath, nameof(fullPath));
            using (TemporaryFilesScopeCookie())
            {
                _fileHistoryProvider.ExecuteWithFileAtRevision(fullPath, revision, OpenFileInEditor);
            }
        }

        public void OpenFileDiff(string fullPath, Revision baseRevision, Revision? changedRevision = null)
        {
            ThrowIf.Null(fullPath, nameof(fullPath));
            var name = Path.GetFileName(fullPath);
            var diffOptions = __VSDIFFSERVICEOPTIONS.VSDIFFOPT_LeftFileIsTemporary;
            if (changedRevision != null)
            {
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                diffOptions |= __VSDIFFSERVICEOPTIONS.VSDIFFOPT_RightFileIsTemporary;
            }

            using (TemporaryFilesScopeCookie())
            {
                _fileHistoryProvider.ExecuteWithFileAtRevision(fullPath, baseRevision, baseFile =>
                {
                    _fileHistoryProvider.ExecuteWithFileAtRevision(fullPath, changedRevision, changedFile =>
                    {
                        _vsDifferenceService.OpenComparisonWindow2(
                            leftFileMoniker: baseFile,
                            rightFileMoniker: changedFile,
                            caption: $"Diff - {name}",
                            Tooltip: $"{name}: current revision - changed",
                            leftLabel: $"{name}: at current revision",
                            rightLabel: $"{name}: changed version",
                            inlineLabel: $"{name}: current revision - changed",
                            roles: null,
                            grfDiffOptions: (uint) diffOptions);
                    });
                });
            }
        }

        [NotNull]
        private static NewDocumentStateScope TemporaryFilesScopeCookie()
        {
            return new NewDocumentStateScope(__VSNEWDOCUMENTSTATE.NDS_Provisional,
                VSConstants.NewDocumentStateReason.TeamExplorer);
        }

        private void OpenFileInEditor([NotNull] string selectedItemFilePath)
        {
            var __ = Guid.Empty;
            _uiShellOpenDocument
                .OpenDocumentViaProject(selectedItemFilePath, ref __, out _, out _, out _, out var frame);
            //TODO readonly and readable name
            //var vsTextView = VsShellUtilities.GetTextView(frame);
            frame?.Show();
        }
    }
}
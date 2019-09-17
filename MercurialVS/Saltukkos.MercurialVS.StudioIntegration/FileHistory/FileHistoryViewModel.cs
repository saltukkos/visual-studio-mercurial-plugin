using System.Collections.Generic;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    public class FileHistoryViewModel
    {
        [NotNull]
        private readonly FileHistoryInfo _fileHistoryInfo;

        [NotNull]
        private readonly IOpenFileService _openFileService;

        public FileHistoryViewModel([NotNull] FileHistoryInfo fileHistoryInfo, [NotNull] IOpenFileService openFileService)
        {
            ThrowIf.Null(openFileService, nameof(openFileService));
            ThrowIf.Null(fileHistoryInfo, nameof(fileHistoryInfo));
            ChangeSets = fileHistoryInfo.ChangeSets;
            _fileHistoryInfo = fileHistoryInfo;
            _openFileService = openFileService;
        }

        [NotNull] 
        public IReadOnlyList<ChangeSet> ChangeSets { get; }

        public ChangeSet? SelectedChangeSet { get; set; }

        public void ViewAtRevision()
        {
            var changeSet = SelectedChangeSet;
            if (changeSet is null)
            {
                return;
            }

            _openFileService.OpenFileFromRevision(_fileHistoryInfo.FilePath, new Revision(changeSet.Value.RevisionNumber));
        }

        public void ShowDiffToLocal()
        {
            var changeSet = SelectedChangeSet;
            if (changeSet is null)
            {
                return;
            }

            _openFileService.OpenFileDiff(_fileHistoryInfo.FilePath, new Revision(changeSet.Value.RevisionNumber), Revision.Current);
        }

        public void ShowDiffToParent()
        {
            var changeSet = SelectedChangeSet;
            if (changeSet is null)
            {
                return;
            }

            var revision = new Revision(changeSet.Value.RevisionNumber);
            _openFileService.OpenFileDiff(_fileHistoryInfo.FilePath, revision.Parent, revision);
        }
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.MercurialVS.SourceControl;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    public class FileHistoryViewModel : INotifyPropertyChanged
    {
        [NotNull]
        private readonly FileHistoryInfo _fileHistoryInfo;

        [NotNull]
        private readonly IOpenFileService _openFileService;

        [NotNull]
        private readonly IFileHistoryProvider _fileHistoryProvider;

        private ChangeSet? _selectedChangeSet;

        [NotNull]
        private IReadOnlyList<DiffLine> _diffLines = new DiffLine[0];

        [NotNull]
        private IReadOnlyList<AnnotationLine> _annotationLines = new AnnotationLine[0];

        public FileHistoryViewModel(
            [NotNull] FileHistoryInfo fileHistoryInfo,
            [NotNull] IOpenFileService openFileService,
            [NotNull] IFileHistoryProvider fileHistoryProvider)
        {
            ThrowIf.Null(fileHistoryProvider, nameof(fileHistoryProvider));
            ThrowIf.Null(openFileService, nameof(openFileService));
            ThrowIf.Null(fileHistoryInfo, nameof(fileHistoryInfo));
            ChangeSets = fileHistoryInfo.ChangeSets;
            _fileHistoryInfo = fileHistoryInfo;
            _openFileService = openFileService;
            _fileHistoryProvider = fileHistoryProvider;
        }

        [NotNull] public IReadOnlyList<ChangeSet> ChangeSets { get; }

        public ChangeSet? SelectedChangeSet
        {
            get => _selectedChangeSet;
            set
            {
                _selectedChangeSet = value;
                UpdateDiffLines(value);
                OnPropertyChanged();
            }
        }

        [NotNull]
        public IReadOnlyList<DiffLine> DiffLines
        {
            get => _diffLines;
            private set
            {
                _diffLines = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public IReadOnlyList<AnnotationLine> AnnotationLines
        {
            get => _annotationLines;
            private set
            {
                _annotationLines = value;
                OnPropertyChanged();
            }
        }

        public void ViewAtRevision()
        {
            var changeSet = SelectedChangeSet;
            if (changeSet is null)
            {
                return;
            }

            _openFileService.OpenFileFromRevision(_fileHistoryInfo.FilePath,
                new Revision(changeSet.Value.RevisionNumber));
        }

        public void ShowDiffToLocal()
        {
            var changeSet = SelectedChangeSet;
            if (changeSet is null)
            {
                return;
            }

            _openFileService.OpenFileDiff(_fileHistoryInfo.FilePath, new Revision(changeSet.Value.RevisionNumber),
                Revision.Current);
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateDiffLines(ChangeSet? changeSet)
        {
            if (changeSet is null)
            {
                DiffLines = new DiffLine[0];
                AnnotationLines = new AnnotationLine[0];
            }
            else
            {
                var revision = new Revision(changeSet.Value.RevisionNumber);
                var path = _fileHistoryInfo.FilePath;
                DiffLines = _fileHistoryProvider.GetDiffToParent(path, revision) ?? new DiffLine[0];
                AnnotationLines = _fileHistoryProvider.AnnotateAtRevision(path, revision) ?? new AnnotationLine[0];
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.SourceControl;
using Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    [Component(typeof(PackageScope))]
    public class FileHistoryViewService : IFileHistoryViewService
    {
        private int _lastFileId;

        [NotNull]
        private readonly Dictionary<string, int> _fileIds = new Dictionary<string, int>();

        [NotNull]
        private readonly IToolWindowContainer _toolWindowContainer;

        [NotNull]
        private readonly IFileHistoryProvider _fileHistoryProvider;

        public FileHistoryViewService(
            [NotNull] IToolWindowContainer toolWindowContainer,
            [NotNull] IFileHistoryProvider fileHistoryProvider)
        {
            ThrowIf.Null(fileHistoryProvider, nameof(fileHistoryProvider));
            ThrowIf.Null(toolWindowContainer, nameof(toolWindowContainer));
            _toolWindowContainer = toolWindowContainer;
            _fileHistoryProvider = fileHistoryProvider;
        }

        public bool ShowHistoryFor(string filePath)
        {
            ThrowIf.Null(filePath, nameof(filePath));

            var fileChangesHistory = _fileHistoryProvider.GetFileChangesHistory(filePath);
            if (fileChangesHistory is null)
            {
                return false;
            }

            if (!_fileIds.TryGetValue(filePath, out var id))
            {
                var newId = Interlocked.Increment(ref _lastFileId);
                _fileIds.Add(filePath, newId);
                id = newId;
            }

            var fileHistoryToolWindow = (FileHistoryToolWindow)_toolWindowContainer.FindToolWindow(typeof(FileHistoryToolWindow), id, create: true);
            fileHistoryToolWindow?.SetFileViewModel(filePath, new FileHistoryViewModel(fileChangesHistory));
            fileHistoryToolWindow?.Show();
            return true;
        }
    }
}
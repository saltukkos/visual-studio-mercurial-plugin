using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.MercurialVS.SourceControl;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Component(typeof(PackageScope))]
    public sealed class GlyphController : IDisposable, IGlyphController
    {
        [NotNull]
        private readonly IDirectoryStateProvider _directoryStateProvider;

        [NotNull]
        private readonly IVsIdleNotifier _idleNotifier;

        [NotNull]
        private readonly IVsSolution _solution;

        [CanBeNull]
        private Dictionary<string, FileStatus> _fileStatuses;

        private bool _glyphsOutdated;

        public GlyphController(
            [NotNull] IDirectoryStateProvider directoryStateProvider,
            [NotNull] IVsIdleNotifier idleNotifier,
            [NotNull] IVsSolution solution)
        {
            ThrowIf.Null(directoryStateProvider, nameof(directoryStateProvider));
            ThrowIf.Null(idleNotifier, nameof(idleNotifier));
            ThrowIf.Null(solution, nameof(solution));

            _directoryStateProvider = directoryStateProvider;
            _idleNotifier = idleNotifier;
            _solution = solution;

            _directoryStateProvider.DirectoryStateChanged += OnDirectoryChanged;
            _idleNotifier.IdlingStarted += UpdateGlyphsState;
        }

        public void Dispose()
        {
            _idleNotifier.IdlingStarted -= UpdateGlyphsState;
        }

        public int GetSccGlyph(string[] paths, VsStateIcon[] glyphs, uint[] sccStatus)
        {
            ThrowIf.Null(glyphs, nameof(glyphs));
            ThrowIf.Null(paths, nameof(paths));

            Trace.WriteLine($"{DateTime.Now}: Got glyphs for {paths.Length} files");
            if (_fileStatuses == null || _fileStatuses.Count == 0)
            {
                return VSConstants.S_FALSE;
            }
            
            for (var i = 0; i < paths.Length; ++i)
            {
                var path = paths[i];
                Debug.Assert(path != null, nameof(path) + " != null");
                if (_fileStatuses.TryGetValue(path.ToUpper(), out var fileStatus))
                {
                    glyphs[i] = ToVsGlyph(fileStatus);
                    if (sccStatus != null)
                        sccStatus[i] = (uint) __SccStatus.SCC_STATUS_CONTROLLED;
                }
                else
                {
                    glyphs[i] = VsStateIcon.STATEICON_BLANK;
                }
            }

            return VSConstants.S_OK;
        }

        [Pure]
        private VsStateIcon ToVsGlyph(FileStatus fileStatus)
        {
            switch (fileStatus)
            {
                case FileStatus.Unknown:
                case FileStatus.Added:
                    return VsStateIcon.STATEICON_ORPHANED;
                case FileStatus.Removed:
                case FileStatus.Missing:
                    return VsStateIcon.STATEICON_CHECKEDOUT;
                case FileStatus.Modified:
                    return VsStateIcon.STATEICON_EDITABLE;
                case FileStatus.Clean:
                    return VsStateIcon.STATEICON_CHECKEDIN;
                case FileStatus.Ignored:
                    return VsStateIcon.STATEICON_DISABLED;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileStatus), fileStatus, null);
            }
        }

        private void OnDirectoryChanged(object sender, EventArgs eventArgs)
        {
            Trace.WriteLine($"{DateTime.Now}: Directory changed, schedule");
            _fileStatuses = _directoryStateProvider.CurrentStatus.ToDictionary(f => f.FilePath.ToUpper(), f => f.Status);
            _glyphsOutdated = true;
        }

        private void UpdateGlyphsState()
        {
            if (!_glyphsOutdated)
            {
                return;
            }

            Trace.WriteLine($"{DateTime.Now}: Lets update all glyphs");

            foreach (var project in GetLoadedControllableProjects())
            {
                project.SccGlyphChanged(0, null, null, null);
            }

            _glyphsOutdated = false;
        }

        /// <summary>
        /// Returns a list of controllable projects in the solution
        /// </summary>
        [NotNull]
        [ItemNotNull]
        private IReadOnlyList<IVsSccProject2> GetLoadedControllableProjects()
        {
            var list = new List<IVsSccProject2>();

            var enumOnlyThisType = new Guid();
            ErrorHandler.ThrowOnFailure(_solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION,
                ref enumOnlyThisType, out var hierarchies));

            var items = new IVsHierarchy[1];
            while (hierarchies.Next(1, items, out var fetched) == VSConstants.S_OK && fetched == 1)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (items[0] is IVsSccProject2 sccProject2)
                {
                    list.Add(sccProject2);
                }
            }

            return list;
        }
    }
}
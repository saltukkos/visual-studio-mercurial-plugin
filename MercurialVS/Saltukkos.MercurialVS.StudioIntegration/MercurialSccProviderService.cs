using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.MercurialVS.SourceControl;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [PackageComponent]
    public sealed class MercurialSccProviderService : IMercurialSccProviderService,
        IVsSccProvider,
        IVsSccManager2
    {
        [NotNull]
        private readonly ISourceControlLifetimeManager _sourceControlLifetimeManager;

        [NotNull]
        private readonly IDirectoryStateProvider _directoryStateProvider;

        [NotNull]
        private readonly IVsSolution _solution;

        [CanBeNull]
        private Dictionary<string, FileStatus> _fileStatuses;

        public MercurialSccProviderService(
            [NotNull] ISourceControlLifetimeManager sourceControlLifetimeManager,
            [NotNull] IDirectoryStateProvider directoryStateProvider,
            [NotNull] IVsSolution solution)
        {
            _sourceControlLifetimeManager = sourceControlLifetimeManager;
            _solution = solution;
            _directoryStateProvider = directoryStateProvider;
            var currentDispatcher = Dispatcher.CurrentDispatcher;
            _directoryStateProvider.DirectoryStateChanged += (sender, args) =>
                currentDispatcher.Invoke(OnDirectoryChanged, DispatcherPriority.Normal);
        }

        private void OnDirectoryChanged()
        {
            _fileStatuses = _directoryStateProvider.CurrentStatus.ToDictionary(f => f.FilePath.ToUpper(), f => f.Status);
            foreach (var project in GetLoadedControllableProjects())
            {
                project.SccGlyphChanged(0, null, null, null);
            }
        }

        public int SetActive()
        {
            _sourceControlLifetimeManager.StartLifetime();
            return VSConstants.S_OK;
        }

        public int SetInactive()
        {
            _sourceControlLifetimeManager.EndLifetime();
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Returns a list of controllable projects in the solution
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public IReadOnlyList<IVsSccProject2> GetLoadedControllableProjects()
        {
            var list = new List<IVsSccProject2>();
            // Hashtable mapHierarchies = new Hashtable();

            var enumOnlyThisType = new Guid();
            ErrorHandler.ThrowOnFailure(_solution.GetProjectEnum((uint) __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION,
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

        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            //TODO not implemented
            pfResult = 1;
            return VSConstants.S_OK;
        }

        public int RegisterSccProject(
            IVsSccProject2 pscp2Project, string pszSccProjectName, string pszSccAuxPath, string pszSccLocalPath,
            string pszProvider)
        {
            return VSConstants.S_OK;
        }

        public int UnregisterSccProject(IVsSccProject2 pscp2Project)
        {
            return VSConstants.S_OK;
        }

        public int GetSccGlyph(int cFiles, string[] rgpszFullPaths, VsStateIcon[] rgsiGlyphs, uint[] rgdwSccStatus)
        {
            if (_fileStatuses == null || _fileStatuses.Count == 0)
            {
                return VSConstants.S_FALSE;
            }
            for (var i = 0; i < cFiles; ++i)
            {
                var rgpszFullPath = rgpszFullPaths[i];
                Debug.Assert(rgpszFullPath != null, nameof(rgpszFullPath) + " != null");
                if (_fileStatuses.TryGetValue(rgpszFullPath.ToUpper(), out var fileStatus))
                {
                    rgsiGlyphs[i] = ToVsGlyph(fileStatus);
                }
                else
                {
                    rgsiGlyphs[i] = VsStateIcon.STATEICON_CHECKEDOUT;
                }
            }

            return VSConstants.S_OK;
        }

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

        public int GetSccGlyphFromStatus(uint dwSccStatus, VsStateIcon[] psiGlyph)
        {
            return VSConstants.S_OK;
        }

        public int IsInstalled(out int pbInstalled)
        {
            pbInstalled = 1;
            return VSConstants.S_OK;
        }

        public int BrowseForProject(out string pbstrDirectory, out int pfOk)
        {
            // Obsolete method
            pbstrDirectory = null;
            pfOk = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int CancelAfterBrowseForProject()
        {
            // Obsolete method
            return VSConstants.E_NOTIMPL;
        }
    }
}
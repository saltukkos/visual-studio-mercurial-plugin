using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Component(typeof(PackageScope))]
    public sealed class MercurialSccProviderService : IMercurialSccProviderService,
        IVsSccProvider,
        IVsSccManager2
    {
        [NotNull] 
        private readonly ILifetimeScopeManager<SourceControlScope> _sourceControlLifetimeManager;

        [NotNull] 
        private readonly IGlyphController _glyphController;

        public MercurialSccProviderService(
            [NotNull] ILifetimeScopeManager<SourceControlScope> sourceControlLifetimeManager,
            [NotNull] IGlyphController glyphController)
        {
            ThrowIf.Null(sourceControlLifetimeManager, nameof(sourceControlLifetimeManager));
            ThrowIf.Null(glyphController, nameof(glyphController));
            _sourceControlLifetimeManager = sourceControlLifetimeManager;
            _glyphController = glyphController;
        }

        public int SetActive()
        {
            _sourceControlLifetimeManager.StartScopeLifetime();
            return VSConstants.S_OK;
        }

        public int SetInactive()
        {
            _sourceControlLifetimeManager.StartScopeLifetime();
            return VSConstants.S_OK;
        }

        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            //TODO not implemented
            pfResult = 1;
            return VSConstants.S_OK;
        }

        public int RegisterSccProject(IVsSccProject2 pscp2Project, string pszSccProjectName, string pszSccAuxPath,
            string pszSccLocalPath, string pszProvider)
        {
            return VSConstants.S_OK;
        }

        public int UnregisterSccProject(IVsSccProject2 pscp2Project)
        {
            return VSConstants.S_OK;
        }

        public int GetSccGlyph(int cFiles, string[] rgpszFullPaths, VsStateIcon[] rgsiGlyphs, uint[] rgdwSccStatus)
        {
            if (rgpszFullPaths == null || rgsiGlyphs == null)
            {
                return VSConstants.S_FALSE;
            }

            return _glyphController.GetSccGlyph(rgpszFullPaths, rgsiGlyphs, rgdwSccStatus);
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
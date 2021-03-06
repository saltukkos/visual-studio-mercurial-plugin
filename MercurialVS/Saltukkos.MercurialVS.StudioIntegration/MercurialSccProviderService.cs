﻿using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.StudioIntegration.Glyphs;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Component(typeof(PackageScope))]
    public sealed class MercurialSccProviderService : IMercurialSccProviderService,
        IVsSccProvider,
        IVsSccGlyphs,
        IVsSccManager2
    {
        [NotNull] 
        private readonly ILifetimeScopeManager<SourceControlScope, None> _sourceControlLifetimeManager;

        [NotNull] 
        private readonly IGlyphController _glyphController;

        [NotNull]
        private readonly ICustomGlyphsProviderInitializer _customGlyphsProviderInitializer;
        
        public MercurialSccProviderService(
            [NotNull] ILifetimeScopeManager<SourceControlScope, None> sourceControlLifetimeManager,
            [NotNull] IGlyphController glyphController,
            [NotNull] ICustomGlyphsProviderInitializer customGlyphsProviderInitializer)
        {
            ThrowIf.Null(customGlyphsProviderInitializer, nameof(customGlyphsProviderInitializer));
            ThrowIf.Null(sourceControlLifetimeManager, nameof(sourceControlLifetimeManager));
            ThrowIf.Null(glyphController, nameof(glyphController));
            _sourceControlLifetimeManager = sourceControlLifetimeManager;
            _glyphController = glyphController;
            _customGlyphsProviderInitializer = customGlyphsProviderInitializer;
        }

        public int SetActive()
        {
            _sourceControlLifetimeManager.StartScopeLifetime(new None());
            return VSConstants.S_OK;
        }

        public int SetInactive()
        {
            _sourceControlLifetimeManager.EndScopeLifetime();
            return VSConstants.S_OK;
        }

        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            //TODO not implemented
            pfResult = 0;
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

        public int GetCustomGlyphList(uint baseIndex, out uint pdwImageListHandle)
        {
            return _customGlyphsProviderInitializer.GetCustomGlyphList(baseIndex, out pdwImageListHandle);
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
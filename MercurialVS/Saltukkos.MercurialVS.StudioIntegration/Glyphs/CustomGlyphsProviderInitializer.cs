using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;

namespace Saltukkos.MercurialVS.StudioIntegration.Glyphs
{
    [Component(typeof(PackageScope))]
    public class CustomGlyphsProviderInitializer : ICustomGlyphsProviderInitializer, ICustomGlyphsProvider
    {
        [NotNull]
        private readonly uint[] _glyphIndexes;

        [CanBeNull]
        private ImageList _glyphList;

        public CustomGlyphsProviderInitializer()
        {
            _glyphIndexes = new[]
            {
                (uint) VsStateIcon.STATEICON_ORPHANED,
                (uint) VsStateIcon.STATEICON_ORPHANED,
                (uint) VsStateIcon.STATEICON_CHECKEDOUT,
                (uint) VsStateIcon.STATEICON_EDITABLE,
                (uint) VsStateIcon.STATEICON_CHECKEDIN
            }; // fallback values in case custom were not init
        }

        public VsStateIcon GetGlyphIcon(Glyph glyph)
        {
            return (VsStateIcon) _glyphIndexes[(int) glyph];
        }

        public int GetCustomGlyphList(uint baseIndex, out uint pdwImageListHandle)
        {
            if (_glyphList != null)
            {
                pdwImageListHandle = (uint) _glyphList.Handle;
                return VSConstants.S_OK;
            }

            if (Resources.Images.EmptyPlus is null ||
                Resources.Images.Plus is null ||
                Resources.Images.RedCross is null ||
                Resources.Images.Pencil is null ||
                Resources.Images.BlueCircle is null)
            {
                pdwImageListHandle = 0;
                return VSConstants.S_OK;
            }

            var glyphList = new ImageList
            {
                ImageSize = new Size(7, 16)
            };

            glyphList.Images.Add(Resources.Images.EmptyPlus);
            glyphList.Images.Add(Resources.Images.Plus);
            glyphList.Images.Add(Resources.Images.RedCross);
            glyphList.Images.Add(Resources.Images.Pencil);
            glyphList.Images.Add(Resources.Images.BlueCircle);

            Debug.Assert(glyphList.Images.Count == Enum.GetValues(typeof(Glyph)).Cast<int>().Max() + 1);
            for (var i = 0; i < glyphList.Images.Count; ++i)
            {
                _glyphIndexes[i] = (uint) (baseIndex + i);
            }
            
            pdwImageListHandle = (uint) glyphList.Handle;
            _glyphList = glyphList;

            return VSConstants.S_OK;
        }
    }
}
using System;
using EnvDTE;
using JetBrains.Annotations;
using Saltukkos.MercurialVS.StudioIntegration.VsServicesWrappers;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.Package.VsServicesWrappers
{
    internal class DteWrapper : IDte
    {
        [NotNull]
        private readonly DTE _dte;

        public DteWrapper([NotNull] DTE dte)
        {
            ThrowIf.Null(dte, nameof(dte));
            _dte = dte;
        }

        public Document ActiveDocument
        {
            get
            {
                try
                {
                    return _dte.ActiveDocument;
                }
                //ActiveDocument throws ArgumentException on some types of documents ¯\_(ツ)_/¯
                catch (ArgumentException)
                {
                    return null;
                }
            }
        }

        public SelectedItems SelectedItems => _dte.SelectedItems;

        public Solution Solution => _dte.Solution;
    }

}
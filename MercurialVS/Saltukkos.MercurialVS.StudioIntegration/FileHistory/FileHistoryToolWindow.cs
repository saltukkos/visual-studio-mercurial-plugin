﻿using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    [Guid(Constants.FileHistoryToolWindowGuid)]
    public class FileHistoryToolWindow : ToolWindowPane
    {
        [NotNull]
        private readonly ElementHost _elementHost;

        public FileHistoryToolWindow()
        {
            //TODO dynamic name
            Caption = "File history";

            _elementHost = new ElementHost
            {
                Dock = DockStyle.Fill
            };
        }

        public override IWin32Window Window => _elementHost;

        public void SetViewModel([NotNull] FileHistoryViewModel fileHistoryViewModel)
        {
            ThrowIf.Null(fileHistoryViewModel, nameof(fileHistoryViewModel));
            _elementHost.Child = new FileHistoryView(fileHistoryViewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _elementHost.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
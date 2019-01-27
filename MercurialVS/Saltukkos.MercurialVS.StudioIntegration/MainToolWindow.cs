using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using JetBrains.Annotations;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Saltukkos.MercurialVS.HgServices;
using Saltukkos.MercurialVS.SourceControl;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Guid(Constants.ToolWindowPaneGuid)]
    public class MainToolWindow : ToolWindowPane
    {
        [NotNull]
        private readonly ElementHost _elementHost;

        [NotNull]
        private readonly SolutionFilesStatusControl _solutionFilesStatusControl = new SolutionFilesStatusControl();

        [NotNull]
        private readonly IDirectoryStateProvider _directoryStateProvider;

        public MainToolWindow()
        {
            Caption = "Main tool window";
            VSColorTheme.ThemeChanged += OnVsColorThemeChanged;
            _elementHost = new ElementHost
            {
                Child = _solutionFilesStatusControl,
                Dock = DockStyle.Fill
            };

            var dependenciesProvider = SccProviderOptionsDependenciesProvider.Instance
                                       ?? throw new InvalidOperationException();
            _directoryStateProvider = dependenciesProvider.DirectoryStateProvider;
            _directoryStateProvider.DirectoryStateChanged += (sender, args) => RefreshFilesList();

            SetDefaultColors();
            RefreshFilesList();
        }

        private void RefreshFilesList()
        {
            _solutionFilesStatusControl.SetFiles(_directoryStateProvider
                .CurrentStatus
                .Where(f => f.Status != FileStatus.Clean && f.Status != FileStatus.Ignored));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                VSColorTheme.ThemeChanged -= OnVsColorThemeChanged;
                _elementHost.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnVsColorThemeChanged(ThemeChangedEventArgs e)
        {
            SetDefaultColors();
        }

        void SetDefaultColors()
        {
            //var defaultBackground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
            //var defaultForeground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);

            //UpdateWindowColors(defaultBackground, defaultForeground);
        }

        public override IWin32Window Window => _elementHost;

        //private void UpdateWindowColors(Color clrBackground, Color clrForeground)
        //{
        //    _control.BackColor = clrBackground;
        //    _control.ForeColor = clrForeground;

        //    foreach (Control child in _control.Controls)
        //    {
        //        child.BackColor = _control.BackColor;
        //        child.ForeColor = _control.ForeColor;
        //    }
        //}
    }
}
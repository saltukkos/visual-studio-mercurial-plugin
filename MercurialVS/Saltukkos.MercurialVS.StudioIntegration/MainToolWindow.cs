using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using JetBrains.Annotations;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    [Guid(Constants.ToolWindowPaneGuid)]
    public class MainToolWindow : ToolWindowPane
    {
        [NotNull]
        private readonly ElementHost _elementHost;

        [NotNull]
        private readonly SolutionFilesStatusViewModel _solutionFilesStatusViewModel;

        public MainToolWindow()
        {
            Caption = "Main tool window";

            var dependenciesProvider = ToolWindowsDependenciesProvider.GetInstance();
            _solutionFilesStatusViewModel = new SolutionFilesStatusViewModel(
                dependenciesProvider.DirectoryStateProvider,
                dependenciesProvider.VsDifferenceService,
                dependenciesProvider.FileHistoryProvider);

            VSColorTheme.ThemeChanged += OnVsColorThemeChanged;
            _elementHost = new ElementHost
            {
                Child = new SolutionFilesStatusView(_solutionFilesStatusViewModel),
                Dock = DockStyle.Fill
            };


            SetDefaultColors();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                VSColorTheme.ThemeChanged -= OnVsColorThemeChanged;
                _elementHost.Dispose();
                _solutionFilesStatusViewModel.Dispose();
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
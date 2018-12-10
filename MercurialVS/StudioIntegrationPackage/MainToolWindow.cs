using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace StudioIntegrationPackage
{
    [Guid(Constants.ToolWindowPaneGuid)]
    public class MainToolWindow : ToolWindowPane
    {
        public MainToolWindow()
        {
            Caption = "Main tool window";
            SetDefaultColors();
            VSColorTheme.ThemeChanged += OnVsColorThemeChanged;
        }

        [NotNull]
        private readonly UserControl _control = new SccProviderOptionsControl();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                VSColorTheme.ThemeChanged -= OnVsColorThemeChanged;
                _control.Dispose();
            }
            base.Dispose(disposing);
        }

        private void OnVsColorThemeChanged(ThemeChangedEventArgs e)
        {
            SetDefaultColors();
        }

        void SetDefaultColors()
        {
            var defaultBackground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
            var defaultForeground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);

            UpdateWindowColors(defaultBackground, defaultForeground);
        }

        public override IWin32Window Window => _control;

        private void UpdateWindowColors(Color clrBackground, Color clrForeground)
        {
            _control.BackColor = clrBackground;
            _control.ForeColor = clrForeground;

            foreach (Control child in _control.Controls)
            {
                child.BackColor = _control.BackColor;
                child.ForeColor = _control.ForeColor;
            }
        }
    }
}
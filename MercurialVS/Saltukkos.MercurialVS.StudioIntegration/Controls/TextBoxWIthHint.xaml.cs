using System.ComponentModel;
using System.Windows;
using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.StudioIntegration.Controls
{
    public partial class TextBoxWithHint : INotifyPropertyChanged
    {
        [NotNull]
        public static readonly DependencyProperty HintProperty = 
            DependencyProperty.Register(nameof(Hint), typeof(string), typeof(TextBoxWithHint), new UIPropertyMetadata(string.Empty, OnHintChanged));

        [NotNull]
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBoxWithHint), new UIPropertyMetadata(string.Empty, OnTextChanged));

        private static void OnHintChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThrowIf.Null(d, nameof(d));
            ((TextBoxWithHint) d).PropertyChanged?.Invoke(d, new PropertyChangedEventArgs(nameof(Hint)));
        }

        private static void OnTextChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThrowIf.Null(d, nameof(d));
            var textBoxWithHint = (TextBoxWithHint) d;
            var hintTextBlock = textBoxWithHint.HintTextBlock;
            ThrowIf.Null(hintTextBlock, nameof(hintTextBlock));

            textBoxWithHint.PropertyChanged?.Invoke(d, new PropertyChangedEventArgs(nameof(Text)));
            hintTextBlock.Visibility = string.IsNullOrEmpty(textBoxWithHint.Text) 
                ? Visibility.Visible 
                : Visibility.Hidden;
        }

        public TextBoxWithHint()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Hint
        {
            get => (string) GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

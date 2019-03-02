using System.ComponentModel;
using System.Windows;
using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.StudioIntegration.Controls
{
    public partial class TextBoxWithHint : INotifyPropertyChanged
    {
        [NotNull]
        public static readonly DependencyProperty HintProperty = 
            DependencyProperty.Register(nameof(Hint), typeof(string), typeof(TextBoxWithHint), new PropertyMetadata(string.Empty));

        [NotNull]
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBoxWithHint), new PropertyMetadata(string.Empty, TextChangedCallBack));

        private static void TextChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TextBoxWithHint)?.PropertyChanged?.Invoke(d, new PropertyChangedEventArgs(nameof(Text)));
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
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

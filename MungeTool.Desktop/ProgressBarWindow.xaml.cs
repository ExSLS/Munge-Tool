using System.Windows;

namespace MungeTool.Desktop
{
    public partial class ProgressBarWindow : Window
    {
        public ProgressBarWindow() =>
            InitializeComponent();

        private void Button_Close(object sender, RoutedEventArgs e) =>
            Close();
    }
}

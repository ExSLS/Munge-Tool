using System.Windows;
using MungeTool.Desktop.ViewModels;

namespace MungeTool.Desktop
{
    /// <summary>
    /// Interaction logic for NewVersionPopup.xaml
    /// </summary>
    public partial class NewVersionPopup : Window
    {
        public NewVersionPopup(NewVersionPopupViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();
    }
}

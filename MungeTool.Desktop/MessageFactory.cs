using System.Windows;

namespace MungeTool.Desktop
{
    class MessageFactory : IMessageFactory
    {
        public void ShowInfoMessage(string message) =>
            MessageBox.Show(message, "", MessageBoxButton.OK, MessageBoxImage.Information);

        public void ShowErrorMessage(string message) =>
            MessageBox.Show(message, "", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

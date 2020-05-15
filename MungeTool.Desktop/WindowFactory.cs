using MungeTool.Desktop.ViewModels;

namespace MungeTool.Desktop
{
    public class WindowFactory : IWindowFactory
    {
       public void CreateProgressBarWindow(ProgressBarViewModel viewModel) =>
            new ProgressBarWindow {
                Owner = MainWindow.Instance,
                DataContext = viewModel,
            }.Show();
    }
}

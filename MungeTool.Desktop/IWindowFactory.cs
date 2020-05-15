using MungeTool.Desktop.ViewModels;

namespace MungeTool.Desktop
{
    public interface IWindowFactory
    {
        void CreateProgressBarWindow(ProgressBarViewModel viewModel);
    }
}

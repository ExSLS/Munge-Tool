using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MungeTool.Lib.Configuration;

namespace MungeTool.Desktop.ViewModels
{
    public class ProgressBarViewModel : INotifyPropertyChanged
    {
        public ICommand OnOpenMungeSolution { get; set; }

        private int _createSlnProgress;
        public int CreateSlnProgress
        {
            get => _createSlnProgress;
            set { _createSlnProgress = value; OnPropertyChanged(); }
        }

        private int _addProjectsToSlnProgress;
        public int AddProjectsToSlnProgress
        {
            get => _addProjectsToSlnProgress;
            set { _addProjectsToSlnProgress = value; OnPropertyChanged(); }
        }

        private int _convertPackageRefsProgress;
        public int ConvertPackageRefsProgress
        {
            get => _convertPackageRefsProgress;
            set { _convertPackageRefsProgress = value; OnPropertyChanged(); }
        }

        private bool _isMunging;
        public bool IsMunging
        {
            get => _isMunging;
            set
            {
                _isMunging = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotMunging));
            }
        }

        public bool IsNotMunging => !IsMunging;

        public ProgressBarViewModel()
        {
            OnOpenMungeSolution = new RelayCommand(x => true, x => OpenMungeSolution());
        }

        private void OpenMungeSolution() =>
            Process.Start(ConfigurationManager.Config.GeneratedMungeSlnFileAbsolute);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

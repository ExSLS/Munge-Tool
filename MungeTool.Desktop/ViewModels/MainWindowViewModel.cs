using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MungeTool.Desktop.Models;
using MungeTool.Lib;
using MungeTool.Lib.Configuration;
using MungeTool.Lib.Models;

namespace MungeTool.Desktop.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string AdosReportBugUri = "https://dev.azure.com/sharp-ls/DevOps-Utilities/_workitems/create/Bug";
        private const string AdosSuggestFeatureUri = "https://dev.azure.com/sharp-ls/DevOps-Utilities/_workitems/create/Product%20Backlog%20Item";

        private readonly IWindowFactory _windowsFactory;
        private readonly IMessageFactory _messageFactory;
        public ICommand OnCreateMungeSolution { get; set; }
        public ICommand OnGitRepoCheckboxChecked { get; set; }
        public ICommand OnGitRepoCheckboxUnChecked { get; set; }

        public List<string> Applications { get; set; }

        private ObservableCollection<ProjectInfo> _projects { get; set; }
        public ObservableCollection<ProjectInfo> Projects { get { return _projects; } set { _projects = value; OnPropertyChanged(); OnPropertyChanged(nameof(NumMungeProjects)); } }

        public int NumMungeProjects => Projects.Count;

        public List<CodeRootFolder> CodeRootFolders { get; set; }

        public ICommand OnReportIssue { get; set; }
        public ICommand OnSuggestFeature { get; set; }

        private string _codeRootFolder;
        public string CodeRootFolder
        {
            get => _codeRootFolder;
            set
            {
                _codeRootFolder = value;
                ConfigurationManager.Config.UserConfig.CodeRootFolder = value;
                ConfigurationManager.SaveUserConfig();
                CodeRootFolders = ConfigurationManager.Config.CodeRootFoldersAbsolute
                    .Select(x => new CodeRootFolder(x, Directory.Exists(x), Directory.Exists(x) && x != ConfigurationManager.Config.AmewodRootFolderAbsolute)).ToList();
                GenerateMungeSolutionList();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CodeRootFolders));
            }
        }

        private string _selectedApplication;
        public string SelectedApplication
        {
            get { return _selectedApplication; }
            set
            {
                _selectedApplication = value;
                GenerateMungeSolutionList();
            }
        }

        private bool _includeTestProjects = true;
        public bool IncludeTestProjects
        {
            get { return _includeTestProjects; }
            set
            {
                _includeTestProjects = value;
                GenerateMungeSolutionList();
            }
        }

        public string MungeToolTitleText => $"MungeTool - v{MungeToolVersionNumber}";

        public string MungeToolVersionNumber =>
            FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public MainWindowViewModel(IWindowFactory windowsFactory, IMessageFactory messageFactory)
        {
            _windowsFactory = windowsFactory;
            _messageFactory = messageFactory;

            OnCreateMungeSolution = new RelayCommand(x => true, x => CreateMungeSolution());
            OnGitRepoCheckboxChecked = new RelayCommand(x => true, x => OnGitRepoCheckboxChanged((string)x, true));
            OnGitRepoCheckboxUnChecked = new RelayCommand(x => true, x => OnGitRepoCheckboxChanged((string)x, false));

            var adosMessage = "Opening ADOS in your browser.\n\nPlease set the 'Area' field to 'MungeTool'";

            OnReportIssue = new RelayCommand(x => true, x =>
            {
                Process.Start(AdosReportBugUri);
                _messageFactory.ShowInfoMessage(adosMessage);
            });

            OnSuggestFeature = new RelayCommand(x => true, x =>
            {
                Process.Start(AdosSuggestFeatureUri);
                _messageFactory.ShowInfoMessage(adosMessage);
            });

            Applications = ConfigurationManager.Config.MainApplications.Select(x => x.Name).ToList();
            CodeRootFolder = ConfigurationManager.Config.UserConfig.CodeRootFolder;

            SelectedApplication = Applications[0];
        }

        private void OnGitRepoCheckboxChanged(string name, bool isChecked)
        {
            GenerateMungeSolutionList();
        }

        private void GenerateMungeSolutionList()
        {
            if (CodeRootFolder == null || SelectedApplication == null)
                return;

            // Check that all checked Git repositories exist
            if (CodeRootFolders
                .Where(x => x.IsIncluded)
                .Any(x => !Directory.Exists(Path.Combine(CodeRootFolder, x.Name))))
                return;

            var generator = new ProjectDependencyCalculator();

            var projectName = SelectedApplication;

            var projectExclusionList = ConfigurationManager.Config.MainApplications
                .Single(x => x.Name == projectName).Exclusions;

            Projects = new ObservableCollection<ProjectInfo>(
                generator.GetAllDependenciesRequiredForProject(
                    CodeRootFolders.Where(x => x.IsIncluded).Select(x => x.FullPath).ToList(), new List<string>(),  projectName, IncludeTestProjects, projectExclusionList));
        }

        public void CreateMungeSolution()
        {
            var progressBarViewModel = new ProgressBarViewModel();

            _windowsFactory.CreateProgressBarWindow(progressBarViewModel);

            progressBarViewModel.IsMunging = true;

            Task.Run(() =>
            {
                var builder = new MungeSolutionBuilder(ConfigurationManager.Config.GeneratedMungeSlnFileAbsolute);

                try
                {
                    builder.Create(Projects.ToList(), (progressValue, progressType) =>
                    {
                        switch (progressType)
                        {
                            case MungeSolutionBuilder.ProgressType.CreateSln:
                                progressBarViewModel.CreateSlnProgress = progressValue;
                                break;
                            case MungeSolutionBuilder.ProgressType.AddProjectsToSln:
                                progressBarViewModel.AddProjectsToSlnProgress = progressValue;
                                break;
                            case MungeSolutionBuilder.ProgressType.ConvertPackageRefsToProjectRefs:
                                progressBarViewModel.ConvertPackageRefsProgress = progressValue;
                                break;
                        }
                    }, null, ConfigurationManager.Config.CodeRootFolders);
                }
                catch (Exception ex)
                {
                    _messageFactory.ShowErrorMessage(ex.Message);
                }
                finally
                {
                    progressBarViewModel.IsMunging = false;
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

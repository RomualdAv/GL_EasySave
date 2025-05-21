using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using V2_WPF_EasySave.Model;
using V2_WPF_EasySave.Utils;
using V2_WPF_EasySave.View;

namespace V2_WPF_EasySave.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged, IJobObserver
    {
        private readonly string _jobDirectory = Path.Combine("..", "..", "..", "SavedJobs");

        public ObservableCollection<JobDef> SavedJobs { get; set; } = new();

        private JobDef? _selectedJob;
        public JobDef? SelectedJob
        {
            get => _selectedJob;
            set
            {
                _selectedJob = value;
                OnPropertyChanged(nameof(SelectedJob));
                OnPropertyChanged(nameof(CanModifyOrDelete));
            }
        }

        public bool CanModifyOrDelete => SelectedJob != null;

        public ICommand RefreshCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand ModifyCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExecuteCommand { get; }

        private JobManager _jobManager = new();

        public MainViewModel()
        {
            _jobManager.RegisterObserver(this);
            
            RefreshCommand = new RelayCommand(_ => LoadSavedJobs());
            CreateCommand = new RelayCommand(_ => OpenJobEditor(null));
            ModifyCommand = new RelayCommand(_ => OpenJobEditor(SelectedJob), _ => CanModifyOrDelete);
            DeleteCommand = new RelayCommand(_ => DeleteSelectedJob(), _ => CanModifyOrDelete);
            ExecuteCommand = new RelayCommand(_ => ExecuteSelectedJob(), _ => CanModifyOrDelete);
            
            LoadSavedJobs();
        }

        public void LoadSavedJobs()
        {
            SavedJobs.Clear();
            var jobs = _jobManager.GetAllSavedJobs();
            foreach (var job in jobs)
                SavedJobs.Add(job);
            SelectedJob = null;
        }

        private void OpenJobEditor(JobDef? jobToEdit)
        {
            var editor = new JobEditorWindow();
            var editorVM = new JobEditorViewModel(jobToEdit);
            editor.DataContext = editorVM;
            editorVM.CloseRequested += (s, saved) => editor.DialogResult = saved;
            editor.ShowDialog();
            if (editor.DialogResult == true)
            {
                _jobManager.SaveJob(editorVM.CurrentJob);
            }
        }

        private void DeleteSelectedJob()
        {
            if (SelectedJob == null) return;
            if (MessageBox.Show($"Supprimer le job '{SelectedJob.Name}' ?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _jobManager.DeleteJob(SelectedJob.Name);
            }
        }

        private void ExecuteSelectedJob()
        {
            if (SelectedJob != null)
            {
                _jobManager.ExecuteJob(SelectedJob);
            }
        }

        
        public void OnJobsChanged()
        {
            LoadSavedJobs();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

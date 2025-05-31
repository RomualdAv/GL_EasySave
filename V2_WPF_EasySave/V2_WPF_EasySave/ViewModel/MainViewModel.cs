using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using V2_WPF_EasySave.Model;
using V2_WPF_EasySave.Utils;
using V2_WPF_EasySave.View;

namespace V2_WPF_EasySave.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged, IJobObserver
    {
        public ObservableCollection<JobDef> SavedJobs { get; set; } = new();
        public ObservableCollection<JobDef> SelectedJobs { get; set; } = new();

        public bool CanModifyOrDelete => SelectedJobs.Any();

        public ICommand RefreshCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand ModifyCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExecuteCommand { get; }
        public ICommand EditBlockedAppsCommand { get; }
        public ICommand EditSizeLimitCommand { get; }

        private JobManager _jobManager = new();

        public MainViewModel()
        {
            _jobManager.RegisterObserver(this);

            SelectedJobs.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(CanModifyOrDelete));
            };

            RefreshCommand = new RelayCommand(_ => LoadSavedJobs());
            CreateCommand = new RelayCommand(_ => OpenJobEditor(null));
            ModifyCommand = new RelayCommand(_ => OpenJobEditor(SelectedJobs.First()), _ => CanModifyOrDelete && SelectedJobs.Count == 1);
            DeleteCommand = new RelayCommand(_ => DeleteSelectedJobs(), _ => CanModifyOrDelete);
            ExecuteCommand = new RelayCommand(_ => ExecuteSelectedJobs(), _ => CanModifyOrDelete);
            EditBlockedAppsCommand = new RelayCommand(_ => OpenBlockedAppsEditor());
            EditSizeLimitCommand = new RelayCommand(_ => OpenSizeLimitSettings());

            LoadSavedJobs();
        }

        public void LoadSavedJobs()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SavedJobs.Clear();

                foreach (var job in _jobManager.GetAllSavedJobs())
                    SavedJobs.Add(job);

                SelectedJobs.Clear();
            });
        }
        
        public void UpdateSelectedJobs(IEnumerable<object> selectedItems)
        {
            SelectedJobs.Clear();

            foreach (var item in selectedItems.OfType<JobDef>())
            {
                SelectedJobs.Add(item);
            }
        }
        
        private void OpenJobEditor(JobDef? jobToEdit)
        {
            var editor = new JobEditorWindow();
            var editorVM = new JobEditorViewModel(jobToEdit);
            editor.DataContext = editorVM;
            editorVM.CloseRequested += (s, saved) => editor.DialogResult = saved;
            editor.ShowDialog();

            if (editor.DialogResult == true)
                _jobManager.SaveJob(editorVM.CurrentJob);
        }

        private void DeleteSelectedJobs()
        {
            if (SelectedJobs.Count == 0) return;

            var names = string.Join(", ", SelectedJobs.Select(j => $"'{j.Name}'"));
            var result = MessageBox.Show($"Supprimer les jobs {names} ?", "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var job in SelectedJobs.ToList())
                    _jobManager.DeleteJob(job.Name);
            }
        }

        private void ExecuteSelectedJobs()
        {
            if (SelectedJobs.Count > 0)
            {
                _jobManager.ExecuteJobsInParallel(SelectedJobs.ToList());
            }
        }

        private void OpenBlockedAppsEditor()
        {
            var window = new BlockedAppsWindow();
            window.ShowDialog();
        }
        
        private void OpenSizeLimitSettings()
        {
            var window = new SizeLimitWindow();
            window.ShowDialog();
        }

        public void OnJobsChanged() => LoadSavedJobs();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

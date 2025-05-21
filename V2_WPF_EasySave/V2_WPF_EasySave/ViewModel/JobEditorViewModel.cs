using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using V2_WPF_EasySave.Model;
using V2_WPF_EasySave.Utils;

namespace V2_WPF_EasySave.ViewModel
{
    public class JobEditorViewModel : INotifyPropertyChanged
    {
        public JobDef CurrentJob { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public JobEditorViewModel(JobDef? job)
        {
            CurrentJob = job == null ? new JobDef() : new JobDef
            {
                Name = job.Name,
                SourceDirectory = job.SourceDirectory,
                TargetDirectory = job.TargetDirectory,
                JobType = job.JobType
            };

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(CurrentJob.Name) ||
                !Directory.Exists(CurrentJob.SourceDirectory) ||
                !Directory.Exists(CurrentJob.TargetDirectory))
            {
                MessageBox.Show("Vérifiez les champs", "Erreur");
                return;
            }
            CloseRequested?.Invoke(this, true);
        }

        private void Cancel() => CloseRequested?.Invoke(this, false);

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public event EventHandler<bool>? CloseRequested;
    }
}
using System.Collections.ObjectModel;
using System.IO;

namespace V2_WPF_EasySave.ViewModel
{
    public class MainViewModel
    {
        public ObservableCollection<string> SavedJobs { get; set; }

        private readonly string _jobDirectory = Path.Combine("..", "..", "..", "SavedJobs");

        public MainViewModel()
        {
            SavedJobs = new ObservableCollection<string>();
            LoadSavedJobs();
        }

        private void LoadSavedJobs()
        {
            if (!Directory.Exists(_jobDirectory))
                Directory.CreateDirectory(_jobDirectory);

            var files = Directory.GetFiles(_jobDirectory, "*.json");
            foreach (var file in files)
            {
                SavedJobs.Add(Path.GetFileNameWithoutExtension(file));
            }
        }
    }
}
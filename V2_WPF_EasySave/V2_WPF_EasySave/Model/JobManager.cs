using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using V2_WPF_EasySave.Model;

namespace V2_WPF_EasySave.Model
{
    public class JobManager
    {
        private readonly string _jobDirectory = Path.Combine("..", "..", "..", "SavedJobs");

        public List<JobDef> GetAllSavedJobs()
        {
            if (!Directory.Exists(_jobDirectory))
                Directory.CreateDirectory(_jobDirectory);

            var files = Directory.GetFiles(_jobDirectory, "*.json");
            var jobs = new List<JobDef>();

            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var job = JsonSerializer.Deserialize<JobDef>(json);
                    if (job != null)
                        jobs.Add(job);
                }
                catch { }
            }

            return jobs;
        }

        public void SaveJob(JobDef job)
        {
            if (!Directory.Exists(_jobDirectory))
                Directory.CreateDirectory(_jobDirectory);

            string path = Path.Combine(_jobDirectory, job.Name + ".json");
            string json = JsonSerializer.Serialize(job, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public void DeleteJob(string jobName)
        {
            string path = Path.Combine(_jobDirectory, jobName + ".json");
            if (File.Exists(path))
                File.Delete(path);
        }

        public void ExecuteJob(JobDef job)
        {
            MessageBox.Show($"Exécution du job lancé.", "Execution", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
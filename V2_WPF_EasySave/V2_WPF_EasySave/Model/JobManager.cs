using System.IO;
using System.Text.Json;
using System.Windows;

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
            try
            {
                var source = job.SourceDirectory;
                var target = job.TargetDirectory;

                if (!Directory.Exists(source))
                {
                    MessageBox.Show("Dossier source introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Directory.CreateDirectory(target); // Creer le dossier si il n’existe pas

                var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);

                foreach (var filePath in files)
                {
                    var relativePath = Path.GetRelativePath(source, filePath);
                    var targetFilePath = Path.Combine(target, relativePath);

                    // Creer les sous-dossiers dans le dossier de destination
                    var targetDirectory = Path.GetDirectoryName(targetFilePath);
                    if (targetDirectory != null && !Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    bool shouldCopy = true;

                    if (job.JobType == 2 && File.Exists(targetFilePath))
                    {
                        var sourceInfo = new FileInfo(filePath);
                        var targetInfo = new FileInfo(targetFilePath);

                        if (sourceInfo.Length == targetInfo.Length &&
                            sourceInfo.LastWriteTime <= targetInfo.LastWriteTime)
                        {
                            shouldCopy = false;
                        }
                    }

                    if (shouldCopy)
                    {
                        File.Copy(filePath, targetFilePath, true);
                    }
                }

                MessageBox.Show($"Le job '{job.Name}' a été exécuté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'exécution du job : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
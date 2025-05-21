using System.IO;
using System.Text.Json;
using System.Windows;
using V2_WPF_EasySave.Utils;
using EasySave.Logging;
using System.Diagnostics;

namespace V2_WPF_EasySave.Model
{
    public class JobManager
    {
        private readonly string _jobDirectory = Path.Combine("..", "..", "..", "SavedJobs");
        private readonly List<IJobObserver> _observers = new();

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
            NotifyJobsChanged();
        }

        public void DeleteJob(string jobName)
        {
            string path = Path.Combine(_jobDirectory, jobName + ".json");
            if (File.Exists(path))
            {
                File.Delete(path);
                NotifyJobsChanged();
            }
        }
        public void ExecuteJob(JobDef job)
        {
            var blockedAppManager = new BlockedAppManager();
            if (blockedAppManager.IsAnyBlockedAppRunning())
            {
                MessageBox.Show("Un logiciel bloquant est en cours d'exécution. Veuillez le fermer avant de lancer le job.", "Exécution bloquée", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var source = job.SourceDirectory;
                var target = job.TargetDirectory;

                if (!Directory.Exists(source))
                {
                    MessageBox.Show("Dossier source introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                var logDirectory = Path.Combine("..", "..", "..", "Logs");
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                var dailyLogManager = new DailyLogManager(logDirectory);
                var stateLogManager = new StateLogManager();

                Directory.CreateDirectory(target);

                var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
                int totalFiles = files.Length;
                long totalSize = files.Sum(f => new FileInfo(f).Length);
                int copiedFiles = 0;
                long copiedSize = 0;

                foreach (var sourceFilePath in files)
                {
                    string relativePath = Path.GetRelativePath(source, sourceFilePath);
                    string targetFilePath = Path.Combine(target, relativePath);
                    string? targetDir = Path.GetDirectoryName(targetFilePath);

                    try
                    {
                        if (!Directory.Exists(targetDir))
                            Directory.CreateDirectory(targetDir);

                        bool shouldCopy = job.JobType == 1 || !File.Exists(targetFilePath);

                        if (job.JobType == 2 && File.Exists(targetFilePath))
                        {
                            DateTime srcTime = File.GetLastWriteTime(sourceFilePath);
                            DateTime dstTime = File.GetLastWriteTime(targetFilePath);
                            shouldCopy = srcTime > dstTime;
                        }

                        if (shouldCopy)
                        {
                            var stopwatch = Stopwatch.StartNew();

                            File.Copy(sourceFilePath, targetFilePath, true);

                            stopwatch.Stop();
                            long fileSize = new FileInfo(sourceFilePath).Length;
                            
                            dailyLogManager.Log(new DailyLog
                            {
                                Timestamp = DateTime.Now.ToString("s"),
                                JobName = job.Name,
                                SourceFilePath = Path.GetFullPath(sourceFilePath),
                                TargetFilePath = Path.GetFullPath(targetFilePath),
                                FileSize = fileSize,
                                TransferTimeMs = stopwatch.ElapsedMilliseconds
                            });
                        }

                        copiedFiles++;
                        copiedSize += new FileInfo(sourceFilePath).Length;
                        
                        stateLogManager.UpdateStateLog(new StateLog
                        {
                            JobName = job.Name,
                            SourceFilePath = sourceFilePath,
                            TargetFilePath = targetFilePath,
                            State = "ACTIVE",
                            TotalFilesToCopy = totalFiles,
                            TotalFilesSize = totalSize,
                            NbFilesLeftToDo = totalFiles - copiedFiles,
                            Progression = (int)((double)copiedFiles / totalFiles * 100)
                        });
                    }
                    catch (Exception ex)
                    {
                        dailyLogManager.Log(new DailyLog
                        {
                            Timestamp = DateTime.Now.ToString("s"),
                            JobName = job.Name,
                            SourceFilePath = Path.GetFullPath(sourceFilePath),
                            TargetFilePath = Path.GetFullPath(targetFilePath),
                            FileSize = 0,
                            TransferTimeMs = -1
                        });
                    }

                    NotifyJobsChanged();
                }
                
                stateLogManager.UpdateStateLog(new StateLog
                {
                    JobName = job.Name,
                    SourceFilePath = "",
                    TargetFilePath = "",
                    State = "END",
                    TotalFilesToCopy = totalFiles,
                    TotalFilesSize = totalSize,
                    NbFilesLeftToDo = 0,
                    Progression = 100
                });

                MessageBox.Show($"Le job '{job.Name}' a été exécuté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'exécution du job : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        
        /* Observer pattern */
        public void RegisterObserver(IJobObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void UnregisterObserver(IJobObserver observer)
        {
            _observers.Remove(observer);
        }

        private void NotifyJobsChanged()
        {
            foreach (var observer in _observers)
                observer.OnJobsChanged();
        }
    }
}
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

        public void ExecuteJobsInParallel(List<JobDef> jobs)
        {
            foreach (var job in jobs)
            {
                var thread = new Thread(() => ExecuteJob(job));
                thread.Start();
            }
        }

        public void ExecuteJob(JobDef job)
        {
            var blockedAppManager = new BlockedAppManager();
            if (blockedAppManager.IsAnyBlockedAppRunning())
            {
                Application.Current.Dispatcher.Invoke(() =>
                    MessageBox.Show("A blocking software is running while you attempt to run a job.", "Execution blocked", MessageBoxButton.OK, MessageBoxImage.Warning));
                return;
            }

            try
            {
                var source = job.SourceDirectory;
                var target = job.TargetDirectory;

                if (!Directory.Exists(source))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        MessageBox.Show("Source directory is missing.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error));
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
                    if (job.StopRequested)
                    {
                        job.State = "ARRETE";
                        break;
                    }

                    job.PauseEvent.WaitOne();
                    job.State = "ACTIVE";

                


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

                            string extension = Path.GetExtension(sourceFilePath).ToLower();
                            if (EncryptionSettings.ExtensionsToEncrypt.Contains(extension))
                            {
                                CryptoManager.EncryptFile(sourceFilePath, targetFilePath, EncryptionSettings.Key);
                            }

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

                            copiedFiles++;
                            copiedSize += fileSize;
                        }

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
                    catch (Exception)
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

                if (!job.StopRequested)
                {
                    job.State = "TERMINE";
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

                Application.Current.Dispatcher.Invoke(() =>
                    MessageBox.Show($"Job '{job.Name}' was executed successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information));
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                    MessageBox.Show($"Error while executing job : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }

        public void RegisterObserver(IJobObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        private void NotifyJobsChanged()
        {
            foreach (var observer in _observers)
                observer.OnJobsChanged();
        }

        public void PauseJob(JobDef job)
        {
            job.State = "EN_PAUSE";
            job.PauseEvent.Reset(); // Met en pause le thread
        }

        public void ResumeJob(JobDef job)
        {
            job.State = "ACTIVE";
            job.PauseEvent.Set(); // Relance l'exécution
        }

        public void StopJob(JobDef job)
        {
            job.StopRequested = true;
            job.PauseEvent.Set(); // Débloque si le job était en pause
        }



    }
}

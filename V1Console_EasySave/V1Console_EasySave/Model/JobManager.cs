namespace V1Console_EasySave.Model;

using System.Diagnostics;
using System.Text.Json;
using System.IO;
using EasySave.Logging; //DLL DailyLogManager

public class JobManager
{
    private readonly string _jobDirectory = Path.Combine("..", "..", "..", "SavedJobs");

    public void SaveJob(JobDef newJob)
    {
        if (!Directory.Exists(_jobDirectory))
            Directory.CreateDirectory(_jobDirectory);

        string fileName = Path.Combine(_jobDirectory, $"{newJob.Name}.json");
        string json = JsonSerializer.Serialize(newJob, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, json);
    }

    public int GetJobCount()
    {
        if (!Directory.Exists(_jobDirectory))
            return 0;

        return Directory.GetFiles(_jobDirectory, "*.json").Length;
    }

    public List<JobDef> GetAllSavedJobs()
    {
        List<JobDef> jobs = new();

        if (!Directory.Exists(_jobDirectory))
            return jobs;

        foreach (var file in Directory.GetFiles(_jobDirectory, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(file);
                var job = JsonSerializer.Deserialize<JobDef>(json);
                if (job != null)
                    jobs.Add(job);
            }
            catch { /* Ignorer les erreurs de parsing */ }
        }

        return jobs;
    }

    public void ExecuteJob(JobDef job)
    {
        if (!Directory.Exists(job.SourceDirectory))
            return;

        var logDirectory = Path.Combine("..", "..", "..", "Logs");
        var dailyLogManager = new DailyLogManager(logDirectory);

        var stateLogManager = new StateLogManager();

        string[] files = Directory.GetFiles(job.SourceDirectory, "*", SearchOption.AllDirectories);

        int totalFiles = files.Length;
        long totalSize = files.Sum(f => new FileInfo(f).Length);
        int copiedFiles = 0;
        long copiedSize = 0;

        foreach (var sourceFilePath in files)
        {
            string relativePath = Path.GetRelativePath(job.SourceDirectory, sourceFilePath);
            string targetFilePath = Path.Combine(job.TargetDirectory, relativePath);
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
                // Console.WriteLine($"Erreur : {ex.Message}");
            }
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
    }
}

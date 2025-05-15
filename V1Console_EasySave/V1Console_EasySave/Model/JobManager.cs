namespace V1Console_EasySave.Model;
using System.Text.Json;
using System.IO;

public class JobManager
{
    private readonly string _jobDirectory = Path.Combine("..", "..", "..", "SavedJobs");

    public void SaveJob(JobDef newJob)
    {
        if (!Directory.Exists(_jobDirectory))
        {
            Directory.CreateDirectory(_jobDirectory);
        }

        string fileName = Path.Combine(_jobDirectory, $"{newJob.Name}.json");
        string json = JsonSerializer.Serialize(newJob, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, json);

        Console.WriteLine($"Job '{newJob.Name}' enregistré dans : {fileName}");
    }

    public int GetJobCount()
    {
        if (!Directory.Exists(_jobDirectory))
        {
            return 0;
        }

        string[] files = Directory.GetFiles(_jobDirectory, "*.json");
        return files.Length;
    }

    public List<JobDef> GetAllSavedJobs()
    {
        List<JobDef> jobs = new List<JobDef>();

        if (!Directory.Exists(_jobDirectory))
            return jobs;

        string[] files = Directory.GetFiles(_jobDirectory, "*.json");

        foreach (var file in files)
        {
            try
            {
                string json = File.ReadAllText(file);
                JobDef job = JsonSerializer.Deserialize<JobDef>(json);

                if (job != null)
                {
                    jobs.Add(job);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Erreur lecture fichier {file} : {ex.Message}");
            }
        }

        return jobs;
    }

   public void ExecuteJob(JobDef job)
    {
        if (!Directory.Exists(job.SourceDirectory))
        {
            return;
        }

        string[] files = Directory.GetFiles(job.SourceDirectory, "*", SearchOption.AllDirectories);

        var stateLogManager = new StateLogManager();

        int totalFiles = files.Length;
        long totalSize = files.Sum(f => new FileInfo(f).Length);
        int copiedFiles = 0;
        long copiedSize = 0;

        foreach (var sourceFilePath in files)
        {
            try
            {
                string relativePath = Path.GetRelativePath(job.SourceDirectory, sourceFilePath);
                string targetFilePath = Path.Combine(job.TargetDirectory, relativePath);
                string? targetDir = Path.GetDirectoryName(targetFilePath);

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                if (job.JobType == 1 || !File.Exists(targetFilePath))
                {
                    File.Copy(sourceFilePath, targetFilePath, true);
                    Console.WriteLine($"Copied : {relativePath}");
                }
                else if (job.JobType == 2)
                {
                    DateTime srcTime = File.GetLastWriteTime(sourceFilePath);
                    DateTime dstTime = File.Exists(targetFilePath) ? File.GetLastWriteTime(targetFilePath) : DateTime.MinValue;

                    if (srcTime > dstTime)
                    {
                        File.Copy(sourceFilePath, targetFilePath, true);
                        Console.WriteLine($"Updated : {relativePath}");
                    }
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
                // Console.WriteLine($"Erreur copie fichier : {ex.Message}");
            }
        }

        // Met à jour l'état final
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

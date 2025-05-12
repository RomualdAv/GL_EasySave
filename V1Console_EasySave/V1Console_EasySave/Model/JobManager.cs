namespace V1Console_EasySave.Model;
using System.Text.Json;
using System.IO;

public class JobManager
{
    private readonly string JobDirectory = Path.Combine("..", "..", "..", "SavedJobs");
    
    public void SaveJob(JobDef newJob)
    {
        if (!Directory.Exists(JobDirectory))
        {
            Directory.CreateDirectory(JobDirectory);
        }
        string fileName = Path.Combine(JobDirectory, $"{newJob.Name}.json");
        string json = JsonSerializer.Serialize(newJob, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, json);
        Console.WriteLine($"Job '{newJob.Name}' enregistré dans : {fileName}");
    }
    
    public int GetJobCount()
    {
        if (!Directory.Exists(JobDirectory))
        {
            return 0;
        }
        string[] files = Directory.GetFiles(JobDirectory, "*.json");
        return files.Length;
    }
    
    public List<string> GetSavedJobNames()
    {
        List<string> jobNames = new List<string>();

        if (!Directory.Exists(JobDirectory))
            return jobNames;

        string[] files = Directory.GetFiles(JobDirectory, "*.json");

        foreach (var file in files)
        {
            try
            {
                string json = File.ReadAllText(file);
                JobDef job = JsonSerializer.Deserialize<JobDef>(json);
                if (job != null && !string.IsNullOrWhiteSpace(job.Name))
                {
                    jobNames.Add(job.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lecture fichier {file} : {ex.Message}");
            }
        }
        return jobNames;
    }
    
    public List<JobDef> GetAllSavedJobs()
    {
        List<JobDef> jobs = new List<JobDef>();

        if (!Directory.Exists(JobDirectory))
            return jobs;

        string[] files = Directory.GetFiles(JobDirectory, "*.json");

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
                Console.WriteLine($"Erreur lecture fichier {file} : {ex.Message}");
            }
        }
        return jobs;
    }
    
    public void ExecuteJob(JobDef job)
    {
        if (!Directory.Exists(job.SourceDirectory))
        {
            Console.WriteLine($"Source introuvable : {job.SourceDirectory}");
            return;
        }

        if (!Directory.Exists(job.TargetDirectory))
        {
            Directory.CreateDirectory(job.TargetDirectory);
        }

        string[] files = Directory.GetFiles(job.SourceDirectory, "*", SearchOption.AllDirectories);

        foreach (var sourceFilePath in files)
        {
            try
            {
                // Déterminer le chemin relatif du fichier
                string relativePath = Path.GetRelativePath(job.SourceDirectory, sourceFilePath);
                string targetFilePath = Path.Combine(job.TargetDirectory, relativePath);
                string? targetDir = Path.GetDirectoryName(targetFilePath);

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // Si type complet ou fichier absent, on copie
                if (job.JobType == 1 || !File.Exists(targetFilePath))
                {
                    File.Copy(sourceFilePath, targetFilePath, true);
                    Console.WriteLine($"Copié : {relativePath}");
                }
                else if (job.JobType == 2)
                {
                    // Job différentiel : on copie seulement si le fichier a été modifié
                    DateTime srcTime = File.GetLastWriteTime(sourceFilePath);
                    DateTime dstTime = File.Exists(targetFilePath) ? File.GetLastWriteTime(targetFilePath) : DateTime.MinValue;

                    if (srcTime > dstTime)
                    {
                        File.Copy(sourceFilePath, targetFilePath, true);
                        Console.WriteLine($"Mis à jour : {relativePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur copie {sourceFilePath} : {ex.Message}");
            }
        }

        Console.WriteLine($"Job '{job.Name}' exécuté avec succès.\n");
    }
}
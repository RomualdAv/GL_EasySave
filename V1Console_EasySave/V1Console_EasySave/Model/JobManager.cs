namespace V1Console_EasySave.Model;
using System.Text.Json;
using System.IO;

public class JobManager
{
    // Path where job will be saved
    private readonly string _jobDirectory = Path.Combine("..", "..", "..", "SavedJobs");
    
    // Save a job
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
    
    // Return the total number of saved jobs
    public int GetJobCount()
    {
        if (!Directory.Exists(_jobDirectory))
        {
            return 0;
        }

        string[] files = Directory.GetFiles(_jobDirectory, "*.json");
        return files.Length;
    }
    
    // Return the list of all saved job objects
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
    
    // Execute a job
    public void ExecuteJob(JobDef job)
    {
        if (!Directory.Exists(job.SourceDirectory))
        {
            // Source directory not found
            return;
        }

        // Get all files from source directory (including subdirectories)
        string[] files = Directory.GetFiles(job.SourceDirectory, "*", SearchOption.AllDirectories);

        foreach (var sourceFilePath in files)
        {
            try
            {
                // Get the relative path of the file to recreate folder structure
                string relativePath = Path.GetRelativePath(job.SourceDirectory, sourceFilePath);
                string targetFilePath = Path.Combine(job.TargetDirectory, relativePath);
                string? targetDir = Path.GetDirectoryName(targetFilePath);

                // Create target subdirectory if needed
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // Copy for full job or if file doesn't exist in target
                if (job.JobType == 1 || !File.Exists(targetFilePath))
                {
                    File.Copy(sourceFilePath, targetFilePath, true);
                    Console.WriteLine($"Copied : {relativePath}");
                }
                else if (job.JobType == 2)
                {
                    // Differential copy: only copy if source is newer than target
                    DateTime srcTime = File.GetLastWriteTime(sourceFilePath);
                    DateTime dstTime = File.Exists(targetFilePath) ? File.GetLastWriteTime(targetFilePath) : DateTime.MinValue;

                    if (srcTime > dstTime)
                    {
                        File.Copy(sourceFilePath, targetFilePath, true);
                        Console.WriteLine($"Updated : {relativePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Erreur coipie fichier : {ex.Message}");
            }
        }

        // Job execution completed
        // Console.WriteLine($"Job '{job.Name}' executed successfully.\n");
    }
}

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
}
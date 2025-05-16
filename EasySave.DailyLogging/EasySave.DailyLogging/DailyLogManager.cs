using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave.Logging;

public class DailyLogManager
{
    private readonly string _logDirectory;

    public DailyLogManager(string logDirectory = "Logs")
    {
        _logDirectory = logDirectory;
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    public void Log(DailyLog newEntry)
    {
        string fileName = Path.Combine(_logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");

        List<DailyLog> entries = new();

        if (File.Exists(fileName))
        {
            string existingJson = File.ReadAllText(fileName);
            entries = JsonSerializer.Deserialize<List<DailyLog>>(existingJson) ?? new();
        }

        entries.Add(newEntry);

        string json = JsonSerializer.Serialize(entries, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(fileName, json);
    }
    
    public void ClearLogs()
    {
        if (Directory.Exists(_logDirectory))
        {
            foreach (var file in Directory.GetFiles(_logDirectory, "*.json"))
            {
                File.Delete(file);
            }
        }
    }
}
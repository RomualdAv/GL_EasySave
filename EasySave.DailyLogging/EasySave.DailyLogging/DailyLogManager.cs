using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
namespace EasySave.Logging;

public class DailyLogManager
{
    private readonly string _logDirectory;
    private readonly string _format;

    public DailyLogManager(string logDirectory, string format = "JSON")
    {
        _logDirectory = logDirectory;
        _format = format;
        if (!Directory.Exists(_logDirectory))
            Directory.CreateDirectory(_logDirectory);
    }

    public void Log(DailyLog logEntry)
    {
        string fileName = Path.Combine(_logDirectory, $"{DateTime.Now:yyyy-MM-dd}." + (_format == "XML" ? "xml" : "json"));

        if (_format == "JSON")
        {
            List<DailyLog> logs = new();
            if (File.Exists(fileName))
            {
                try
                {
                    string existingJson = File.ReadAllText(fileName);
                    logs = JsonSerializer.Deserialize<List<DailyLog>>(existingJson) ?? new List<DailyLog>();
                }
                catch { }
            }
            logs.Add(logEntry);
            string json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
        }
        else if (_format == "XML")
        {
            List<DailyLog> logs = new();
            if (File.Exists(fileName))
            {
                try
                {
                    using (var stream = File.OpenRead(fileName))
                    {
                        var serializer = new XmlSerializer(typeof(List<DailyLog>));
                        logs = (List<DailyLog>)serializer.Deserialize(stream) ?? new List<DailyLog>();
                    }
                }
                catch { }
            }
            logs.Add(logEntry);
            using (var stream = File.Create(fileName))
            {
                var serializer = new XmlSerializer(typeof(List<DailyLog>));
                serializer.Serialize(stream, logs);
            }
        }
    }

    public void ClearLogs()
    {
        if (Directory.Exists(_logDirectory))
            Directory.Delete(_logDirectory, true);
    }

}
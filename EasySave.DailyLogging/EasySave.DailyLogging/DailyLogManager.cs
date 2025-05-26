using System.Text.Json;
using System.Xml.Serialization;

namespace EasySave.Logging;

public class DailyLogManager
{
    private readonly string _logDirectory;
    private readonly string _format;
    
    private static readonly object _lock = new();

    public DailyLogManager(string logDirectory, string format = "JSON")
    {
        _logDirectory = logDirectory;
        _format = format.ToUpperInvariant();

        if (!Directory.Exists(_logDirectory))
            Directory.CreateDirectory(_logDirectory);
    }

    public void Log(DailyLog logEntry)
    {
        string fileName = Path.Combine(_logDirectory, $"{DateTime.Now:yyyy-MM-dd}.{(_format == "XML" ? "xml" : "json")}");

        lock (_lock)
        {
            List<DailyLog> logs = new();

            if (File.Exists(fileName))
            {
                try
                {
                    if (_format == "JSON")
                    {
                        string existingJson = File.ReadAllText(fileName);
                        logs = JsonSerializer.Deserialize<List<DailyLog>>(existingJson) ?? new();
                    }
                    else if (_format == "XML")
                    {
                        using var stream = File.OpenRead(fileName);
                        var serializer = new XmlSerializer(typeof(List<DailyLog>));
                        logs = (List<DailyLog>)serializer.Deserialize(stream)! ?? new();
                    }
                }
                catch
                {
                    logs = new();
                }
            }

            logs.Add(logEntry);

            try
            {
                if (_format == "JSON")
                {
                    string json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(fileName, json);
                }
                else if (_format == "XML")
                {
                    using var stream = File.Create(fileName);
                    var serializer = new XmlSerializer(typeof(List<DailyLog>));
                    serializer.Serialize(stream, logs);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Erreur d’écriture du log : {ex.Message}");
            }
        }
    }

    public void ClearLogs()
    {
        lock (_lock)
        {
            if (Directory.Exists(_logDirectory))
                Directory.Delete(_logDirectory, true);
        }
    }
}

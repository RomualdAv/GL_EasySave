using System.Text.Json;
using System.IO;

namespace V2_WPF_EasySave.Model;

public class StateLogManager
{
    private readonly string _stateLogPath = Path.Combine("..", "..", "..", "Logs", "state.json");
    
    private static readonly object _lock = new();

    public StateLogManager()
    {
        var dir = Path.GetDirectoryName(_stateLogPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir!);
        }
    }

    /// <summary>
    /// Met à jour ou ajoute l'état d'un job de sauvegarde dans state.json
    /// </summary>
    public void UpdateStateLog(StateLog newEntry)
    {
        lock (_lock)
        {
            List<StateLog> entries = new();

            try
            {
                if (File.Exists(_stateLogPath))
                {
                    string existingJson = File.ReadAllText(_stateLogPath);
                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        entries = JsonSerializer.Deserialize<List<StateLog>>(existingJson) ?? new();
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Erreur de lecture JSON: {ex.Message}");
                entries = new();
            }
            
            int index = entries.FindIndex(e => e.JobName == newEntry.JobName);
            if (index >= 0)
            {
                entries[index] = newEntry;
            }
            else
            {
                entries.Add(newEntry);
            }
            
            string updatedJson = JsonSerializer.Serialize(entries, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_stateLogPath, updatedJson);
        }
    }

    /// <summary>
    /// Supprime le fichier state.json
    /// </summary>
    public void ClearStateLog()
    {
        lock (_lock)
        {
            if (File.Exists(_stateLogPath))
            {
                File.Delete(_stateLogPath);
            }
        }
    }
}

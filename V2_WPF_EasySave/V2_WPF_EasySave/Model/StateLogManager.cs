using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace V2_WPF_EasySave.Model;

public class StateLogManager
{
    private readonly string _stateLogPath = Path.Combine("..", "..", "..", "Logs", "state.json");

    public StateLogManager()
    {
        // Create directory if it doesn't exist
        var dir = Path.GetDirectoryName(_stateLogPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    // Update the state of a specific job in the list
    public void UpdateStateLog(StateLog newEntry)
    {
        List<StateLog> entries = new List<StateLog>();

        if (File.Exists(_stateLogPath))
        {
            string existingJson = File.ReadAllText(_stateLogPath);
            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                try
                {
                    entries = JsonSerializer.Deserialize<List<StateLog>>(existingJson)
                              ?? new List<StateLog>();
                }
                catch
                {
                    entries = new List<StateLog>();
                }
            }
        }

        // Replace existing entry for this job if it exists
        int existingIndex = entries.FindIndex(e => e.JobName == newEntry.JobName);
        if (existingIndex >= 0)
        {
            entries[existingIndex] = newEntry;
        }
        else
        {
            entries.Add(newEntry);
        }

        string updatedJson = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_stateLogPath, updatedJson);
    }

    // Clear all job states
    public void ClearStateLog()
    {
        if (File.Exists(_stateLogPath))
        {
            File.Delete(_stateLogPath);
        }
    }
}
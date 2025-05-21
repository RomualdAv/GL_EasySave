using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace V2_WPF_EasySave.Model;

public class BlockedAppManager
{
    private readonly string _blockedAppsPath = Path.Combine("..", "..", "..", "Settings", "blocked.json");

    public List<string> LoadBlockedApps()
    {
        if (!File.Exists(_blockedAppsPath)) return new List<string>();
        var json = File.ReadAllText(_blockedAppsPath);
        return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
    }

    public void SaveBlockedApps(List<string> apps)
    {
        var json = JsonSerializer.Serialize(apps, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_blockedAppsPath, json);
    }

    public bool IsAnyBlockedAppRunning()
    {
        var blockedApps = LoadBlockedApps();
        var runningProcesses = Process.GetProcesses().Select(p => p.ProcessName.ToLower());
        return blockedApps.Any(app => runningProcesses.Contains(app.ToLower()));
    }
}
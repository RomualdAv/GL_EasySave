using System.Text.Json;
using System.IO;
using System.Windows;

namespace V2_WPF_EasySave.Utils;

public class SizeLimit
{
    private static readonly string ConfigPath = Path.Combine("..", "..", "..", "Settings", "size_limit.json");

    public long MaxParallelFileSizeKB { get; set; } = 10000;

    public static SizeLimit Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<SizeLimit>(json) ?? new SizeLimit();
            }
        }
        catch { }

        return new SizeLimit();
    }

    public void Save()
    {
        var dir = Path.GetDirectoryName(ConfigPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }
}
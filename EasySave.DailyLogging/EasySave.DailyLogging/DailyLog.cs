namespace EasySave.Logging;

public class DailyLog
{
    public string Timestamp { get; set; } = "";
    public string JobName { get; set; } = "";
    public string SourceFilePath { get; set; } = "";
    public string TargetFilePath { get; set; } = "";
    public long FileSize { get; set; }
    public long TransferTimeMs { get; set; }
}
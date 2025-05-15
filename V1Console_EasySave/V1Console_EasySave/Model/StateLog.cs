namespace V1Console_EasySave.Model;

public class StateLog
{
    public string JobName { get; set; }
    public string SourceFilePath { get; set; }
    public string TargetFilePath { get; set; }
    public string State { get; set; }
    public int TotalFilesToCopy { get; set; }
    public long TotalFilesSize { get; set; }
    public int NbFilesLeftToDo { get; set; }
    public int Progression { get; set; }
}
namespace V1Console_EasySave.Model;

public class JobDef
{
    public String Name { get; set; }
    public String SourceDirectory { get; set; }
    public String TargetDirectory { get; set; }
    public int JobType { get; set; } // 1 = Complète - 2 = Différentielle
}
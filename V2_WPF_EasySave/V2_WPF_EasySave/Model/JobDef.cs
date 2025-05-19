namespace V2_WPF_EasySave.Model
{
    public class JobDef
    {
        public string Name { get; set; } = string.Empty;
        public string SourceDirectory { get; set; } = string.Empty;
        public string TargetDirectory { get; set; } = string.Empty;
        public int JobType { get; set; } = 1;
    }
}
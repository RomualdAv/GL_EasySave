namespace V1Console_EasySave.ViewModel;
using V1Console_EasySave.Model;

public class Fr_ViewModel
{
    private JobManager _jobManager = new JobManager();
    private JobDef NewJob { get; set; } = new JobDef();

    public void AddJob()
    {
        // Save the job to a file
        _jobManager.SaveJob(NewJob);
    }
    
    public int GetJobsNB()
    {
        return _jobManager.GetJobCount();
    }
    
    public bool SetName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            NewJob.Name = name;
            return true;
        }
        return false;
    }

    public bool SetSourceDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            NewJob.SourceDirectory = path;
            return true;
        }
        return false;
    }

    public bool SetTargetDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            NewJob.TargetDirectory = path;
            return true;
        }
        return false;
    }

    public bool SetJobType(int type)
    {
        if (type == 1 || type == 2)
        {
            NewJob.JobType = type;
            return true;
        }
        return false;
    }
}
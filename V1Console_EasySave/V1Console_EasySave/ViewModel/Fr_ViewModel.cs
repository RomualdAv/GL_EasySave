using V1Console_EasySave.Model;

namespace V1Console_EasySave.ViewModel;

public class Fr_ViewModel
{
    private JobManager _jobManager = new JobManager();
    public void CreateJob()
    {
        _jobManager.AddJob();
    }
}
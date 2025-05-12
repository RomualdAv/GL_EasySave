namespace V1Console_EasySave.ViewModel;
using V1Console_EasySave.Model;

public class Fr_ViewModel
{
    private JobManager _jobManager = new JobManager();
    private JobDef NewJob { get; set; } = new JobDef();

    public void AddJob()
    {
        // Save the job to a json file
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
    
    public List<string> GetSavedJobNames()
    {
        return _jobManager.GetSavedJobNames();
    }

    public List<JobDef> GetAllSavedJobs()
    {
        return _jobManager.GetAllSavedJobs();
    }
    
    public List<int> ParseSelection(string input, int max)
    {
        var result = new List<int>();

        if (string.IsNullOrWhiteSpace(input))
            return result;

        if (input.Contains("-"))
        {
            var parts = input.Split('-');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int start) &&
                int.TryParse(parts[1], out int end) &&
                start >= 1 && end <= max && start <= end)
            {
                for (int i = start; i <= end; i++)
                    result.Add(i - 1);
            }
        }
        else
        {
            var parts = input.Split('+');
            foreach (var part in parts)
            {
                if (int.TryParse(part, out int index) && index >= 1 && index <= max)
                {
                    result.Add(index - 1);
                }
            }
        }

        return result.Distinct().ToList();
    }
    
    public void ExecuteJobs(JobDef job)
    {
        _jobManager.ExecuteJob(job);
    }

}
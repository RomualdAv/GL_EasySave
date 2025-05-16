namespace V1Console_EasySave.ViewModel;
using V1Console_EasySave.Model;

public class Console_ViewModel
{
    private JobManager _jobManager = new JobManager();
    private JobDef NewJob { get; set; } = new JobDef();
    private LanguageManager _languageManager = new LanguageManager();

    // Class Constructor
    public Console_ViewModel()
    {
        _languageManager.LoadLastUsedLanguage();
    }

    // Set the language (en/fr)
    public void SetLanguage(string langCode)
    {
        _languageManager.LoadLanguage(langCode);
    }

    // Translate a key using the current language
    public string T(string key)
    {
        return _languageManager.Translate(key);
    }

    // Save the current job
    public void AddJob()
    {
        _jobManager.SaveJob(NewJob);
    }

    // Get the number of saved jobs
    public int GetJobsNB()
    {
        return _jobManager.GetJobCount();
    }

    // Set job name
    public bool SetName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            NewJob.Name = name;
            return true;
        }
        return false;
    }

    // Set source directory if it exists
    public bool SetSourceDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            NewJob.SourceDirectory = path;
            return true;
        }
        return false;
    }

    // Set target directory if it exists
    public bool SetTargetDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            NewJob.TargetDirectory = path;
            return true;
        }
        return false;
    }

    // Set job type (1 = full, 2 = differential)
    public bool SetJobType(int type)
    {
        if (type == 1 || type == 2)
        {
            NewJob.JobType = type;
            return true;
        }
        return false;
    }

    // Get all saved jobs indetails
    public List<JobDef> GetAllSavedJobs()
    {
        return _jobManager.GetAllSavedJobs();
    }

    // Parse the user selection input (e.g., "1+3+5" or "2-4")
    public List<int> ParseSelection(string input, int max)
    {
        var result = new List<int>();

        if (string.IsNullOrWhiteSpace(input))
            return result;

        // Handle range selection like "2-5"
        if (input.Contains("-"))
        {
            var parts = input.Split('-');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int start) &&
                int.TryParse(parts[1], out int end) &&
                start >= 1 && end <= max && start <= end)
            {
                for (int i = start; i <= end; i++)
                    result.Add(i - 1); // Convert to zero-based index
            }
        }
        else // Handle multiple values like "1+3+4"
        {
            var parts = input.Split('+');
            foreach (var part in parts)
            {
                if (int.TryParse(part, out int index) && index >= 1 && index <= max)
                {
                    result.Add(index - 1); // Convert to zero-based index
                }
            }
        }

        return result.Distinct().ToList(); // Remove duplicates
    }

    // Execute a job
    public void ExecuteJobs(JobDef job)
    {
        _jobManager.ExecuteJob(job);
    }
    public void UpdateJob(JobDef job)
    {
        _jobManager.SaveJob(job);

    }

    public void DeleteJob(string jobName)
    {
        _jobManager.DeleteJob(jobName);
    
    }
    
}

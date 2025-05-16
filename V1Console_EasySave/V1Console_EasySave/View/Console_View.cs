using V1Console_EasySave.ViewModel;

namespace V1Console_EasySave.View
{
    public class Console_View
    {
        private Console_ViewModel _consoleViewModel = new Console_ViewModel();

        // Main loop of the console application
        public void Main()
        {
            while (true)
            {
                Console.Clear();
                
                Console.WriteLine(_consoleViewModel.T("Welcome"));
                Console.WriteLine($"1 - {_consoleViewModel.T("CreateJob")}");
                Console.WriteLine($"2 - {_consoleViewModel.T("ExecuteJobs")}");
                Console.WriteLine($"3 - {_consoleViewModel.T("ChangeLanguage")}");
                Console.WriteLine($"4 - {_consoleViewModel.T("ClearDailyLogs")}");
                Console.WriteLine($"5 - {_consoleViewModel.T("Quit")}");
                Console.Write(_consoleViewModel.T("ChooseOption"));

                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        ShowJobCreation();  // Create a new job
                        break;
                    case "2":
                        ShowSavedJobs();    // Show saved jobs
                        ShowJobChoice();    // Allow the user to choose one or multiple jobs
                        break;
                    case "3":
                        ChangeLanguage();   // Change language
                        break;
                    case "4":
                        ClearLogsPrompt();  // Clear daily logs
                        break;
                    case "5":
                        Environment.Exit(0); // Exit application
                        break;
                    default:
                        continue; // Invalid input, loop again
                }
            }
        }

        // Change language
        public void ChangeLanguage()
        {
            Console.Write(_consoleViewModel.T("LanguageOptions"));
            string lang = Console.ReadLine();
            _consoleViewModel.SetLanguage(lang); // Set and Save language choice
        }

        // Job creation process
        public void ShowJobCreation()
        {
            string input;

            // Set job name
            do
            {
                Console.Write(_consoleViewModel.T("EnterJobName"));
                input = Console.ReadLine();
            } while (!_consoleViewModel.SetName(input));

            // Set source directory
            do
            {
                Console.Write(_consoleViewModel.T("EnterSourceDirectory"));
                input = Console.ReadLine();
            } while (!_consoleViewModel.SetSourceDirectory(input));

            // Set target directory
            do
            {
                Console.Write(_consoleViewModel.T("EnterTargetDirectory"));
                input = Console.ReadLine();
            } while (!_consoleViewModel.SetTargetDirectory(input));

            // Set job type (1 = full, 2 = differential)
            do
            {
                Console.Write(_consoleViewModel.T("EnterJobType"));
                input = Console.ReadLine();
            } while (!int.TryParse(input, out int type) || !_consoleViewModel.SetJobType(type));

            _consoleViewModel.AddJob(); // Save job

            Console.WriteLine(_consoleViewModel.T("JobCreatedSuccess"));
            Console.ReadKey();
        }

        // Display the saved jobs
        public void ShowSavedJobs()
        {
            var jobs = _consoleViewModel.GetAllSavedJobs();

            Console.WriteLine($"\n{_consoleViewModel.T("SavedJobsList")}");

            if (jobs.Count == 0)
            {
                Console.WriteLine(_consoleViewModel.T("NoJobsFound"));
            }
            else
            {
                for (int i = 0; i < jobs.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {jobs[i].Name}");
                }
            }
        }

        // Use can choose one or multiple jobs to execute
        public void ShowJobChoice()
        {
            var jobs = _consoleViewModel.GetAllSavedJobs();

            if (jobs.Count == 0)
            {
                Console.WriteLine(_consoleViewModel.T("NoJobsToRun"));
                Console.ReadKey();
                return;
            }

            Console.Write($"\n{_consoleViewModel.T("SelectJobsToRun")}");
            string input = Console.ReadLine();
            var selectedIndexes = _consoleViewModel.ParseSelection(input, jobs.Count);

            Console.WriteLine($"\n{_consoleViewModel.T("SelectedJobs")}");

            foreach (int index in selectedIndexes)
            {
                Console.WriteLine($"{_consoleViewModel.T("RunningJob")} {jobs[index].Name}...");
                _consoleViewModel.ExecuteJobs(jobs[index]);
            }

            Console.WriteLine(_consoleViewModel.T("JobsExecutionCompleted"));
            Console.ReadKey();
        }
        
        public void ClearLogsPrompt()
        {
            Console.WriteLine(_consoleViewModel.T("ConfirmClearLogs"));
            string input = Console.ReadLine()?.ToLower();
            if (input == "y" || input == "yes" || input == "o" || input == "oui")
            {
                _consoleViewModel.ClearDailyLogs();
                Console.WriteLine(_consoleViewModel.T("LogsCleared"));
            }
            else
            {
                Console.WriteLine(_consoleViewModel.T("Canceled"));
            }
            Console.ReadKey();
        }
    }
}
using System.Text.Json;
using System.Globalization; 

namespace V1Console_EasySave.View;
using V1Console_EasySave.ViewModel;

public class Fr_View
{
   private Fr_ViewModel _frViewModel = new Fr_ViewModel();
    private string _language;
    private readonly string configFile = "config.json";

    public Fr_View()
    {
        LoadLanguage(); // Charger la langue depuis le fichier JSON
    }

    public void Main()
    {
        Console.Clear();

        if (_language == "FR")
        {
            Console.WriteLine("Bienvenue dans EasySave");
            Console.WriteLine("1. Créer un job");
            Console.WriteLine("2. Quitter");
            Console.WriteLine("3. Changer la langue");
            Console.Write("Choisissez une option : ");
        }
        else
        {
            Console.WriteLine("Welcome to EasySave");
            Console.WriteLine("1. Create a job");
            Console.WriteLine("2. Exit");
            Console.WriteLine("3. Change language");
            Console.Write("Choose an option: ");
        }

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ShowJobCreation();
                break;
            case "2":
                Environment.Exit(0);
                break;
            case "3":
                ChangeLanguage();
                break;
            default:
                Console.WriteLine(_language == "FR" ? "Option invalide." : "Invalid option.");
                Main();
                break;
        }
    }

    private void ChangeLanguage()
    {
        Console.WriteLine("1. Français");
        Console.WriteLine("2. English");
        Console.Write("Choix / Choice: ");
        string input = Console.ReadLine();

        if (input == "1") _language = "FR";
        else if (input == "2") _language = "EN";
        else
        {
            Console.WriteLine(_language == "FR" ? "Entrée invalide." : "Invalid input.");
            return;
        }

        SaveLanguage();
        Main();
    }

    private void LoadLanguage()
{
    if (File.Exists(configFile))
    {
        try
        {
            string json = File.ReadAllText(configFile);
            var config = JsonSerializer.Deserialize<Config>(json);
            _language = config?.Language ?? "FR";
        }
        catch
        {
            _language = "FR";
            SaveLanguage();
        }
    }
    else
    {
        string systemLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
        _language = (systemLang == "EN") ? "EN" : "FR"; // par défaut en FR si ce n'est pas EN
        SaveLanguage();
    }
}

    private void SaveLanguage()
    {
        var config = new Config { Language = _language };
        string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configFile, json);
    }

    public void ShowJobCreation()
    {
        if (_frViewModel.GetJobsNB() >= 5)
        {
            Console.WriteLine(_language == "FR"
                ? "Vous avez atteint le nombre maximum de jobs (5)."
                : "You have reached the maximum number of jobs (5).");
            Main();
            return;
        }

        Console.WriteLine(_language == "FR" ? "Création d'un job..." : "Creating a job...");

        string input;
        do
        {
            Console.Write(_language == "FR" ? "Entrez le nom du job : " : "Enter job name: ");
            input = Console.ReadLine();
        } while (!_frViewModel.SetName(input));

        do
        {
            Console.Write(_language == "FR" ? "Entrez le répertoire source : " : "Enter source directory: ");
            input = Console.ReadLine();
        } while (!_frViewModel.SetSourceDirectory(input));

        do
        {
            Console.Write(_language == "FR" ? "Entrez le répertoire cible : " : "Enter target directory: ");
            input = Console.ReadLine();
        } while (!_frViewModel.SetTargetDirectory(input));

        do
        {
            Console.Write(_language == "FR"
                ? "Entrez le type de job : 1 = Complète - 2 = Différentielle : "
                : "Enter job type: 1 = Full - 2 = Differential: ");
            input = Console.ReadLine();
        } while (!int.TryParse(input, out int type) || !_frViewModel.SetJobType(type));

        _frViewModel.AddJob();
        Main();
    }

    private class Config
    {
        public string Language { get; set; } = "FR";
    }
}

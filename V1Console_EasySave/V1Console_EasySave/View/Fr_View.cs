namespace V1Console_EasySave.View;
using V1Console_EasySave.ViewModel;
public class Fr_View
{
    private Fr_ViewModel _frViewModel = new Fr_ViewModel();
    
    public void Main()
    {
        Console.WriteLine("Bienvenue dans EasySave");
        Console.WriteLine("1. Créer un job");
        Console.WriteLine("2. Quitter");
        Console.Write("Choisissez une option : ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ShowJobCreation();
                break;
            case "2":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Option invalide, veuillez réessayer.");
                Main();
                break;
        }
    }
    
    public void ShowJobCreation()
    {
        Console.WriteLine("Création d'un job...");

        string input;
        do
        {
            Console.Write("Entrez le nom du job : ");
            input = Console.ReadLine();
        } while (!_frViewModel.SetName(input));

        do
        {
            Console.Write("Entrez le répertoire source : ");
            input = Console.ReadLine();
        } while (!_frViewModel.SetSourceDirectory(input));

        do
        {
            Console.Write("Entrez le répertoire cible où seront copiés les fichiers : ");
            input = Console.ReadLine();
        } while (!_frViewModel.SetTargetDirectory(input));

        do
        {
            Console.Write("Entrez le type de job : 1 = Complète - 2 = Différentielle : ");
            input = Console.ReadLine();
        } while (!int.TryParse(input, out int type) || !_frViewModel.SetJobType(type));
        
        _frViewModel.AddJob();
    }
}
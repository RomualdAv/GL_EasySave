using V1Console_EasySave.ViewModel;

namespace V1Console_EasySave.View;

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
                _frViewModel.CreateJob();
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
}
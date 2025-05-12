namespace V1Console_EasySave.Model;
using System.IO;

public class JobManager
{
    private String JobDirectory = "";
    
    public void AddJob()
    {
        Console.WriteLine("Creation d'un job...");
        JobDef newJob = new JobDef();
        Console.Write("Entrez le nom du job : ");
        newJob.Name = Console.ReadLine();
        while (!Directory.Exists(newJob.SourceDirectory))
        {
            Console.Write("Entrez le repertoire source : ");
            newJob.SourceDirectory = Console.ReadLine();
        }
        while (!Directory.Exists(newJob.TargetDirectory))
        {
            Console.Write("Entrez le repertoire cible où seront copié les fichiers : ");
            newJob.TargetDirectory = Console.ReadLine();
        }
        Console.Write("Entrez le type de job : ");
        newJob.JobType = Console.ReadLine();
    }
}
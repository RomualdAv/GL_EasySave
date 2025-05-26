using System.Diagnostics;
using System.IO;

namespace V2_WPF_EasySave.Utils
{
    public static class CryptoManager
    {
        public static void EncryptFile(string inputFilePath, string outputFilePath, string key)
        {
            var tempPath = Path.GetTempFileName(); // chiffre dans un fichier temporaire

            var exePath = Path.Combine("..", "..", "..", "CryptoSoft", "cryptosoft.exe");

            if (!File.Exists(exePath))
            {
                Debug.WriteLine("CryptoSoft introuvable");
                return;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = $"\"{inputFilePath}\" {key}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Copie le fichier chiffré
            if (File.Exists(inputFilePath))
                File.Move(inputFilePath, outputFilePath, true);

            Debug.WriteLine($"CryptoSoft sortie : {output}");
        }
    }
}

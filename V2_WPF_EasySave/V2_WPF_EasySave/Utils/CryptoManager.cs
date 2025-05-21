using System.Diagnostics;
using System.IO;

namespace V2_WPF_EasySave.Utils
{
    public static class CryptoManager
    {
        public static void EncryptFile(string filePath, string key)
        {
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
                    Arguments = $"\"{filePath}\" {key}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            Debug.WriteLine($"CryptoSoft sortie : {output}");
        }
        Debug.WriteLine($"Fichier chiffré : {sourcePath}");

    }
    
}

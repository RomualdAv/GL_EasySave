using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace V2_WPF_EasySave.Utils
{
    public class PriorityExtensions
    {
        private static readonly string filePath = Path.Combine("..", "..", "..", "Settings", "priority_extensions.json");

        public List<string> Extensions { get; set; } = new();

        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(Extensions, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde des extensions prioritaires : {ex.Message}");
            }
        }

        public static PriorityExtensions Load()
        {
            try
            {
                if (!File.Exists(filePath))
                    return new PriorityExtensions();

                string json = File.ReadAllText(filePath);
                var list = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();

                return new PriorityExtensions { Extensions = list };
            }
            catch
            {
                return new PriorityExtensions();
            }
        }
    }
}
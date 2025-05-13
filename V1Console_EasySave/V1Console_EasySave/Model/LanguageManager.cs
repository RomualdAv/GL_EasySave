using System.Text.Json;

namespace V1Console_EasySave.Model
{
    public class LanguageManager
    {
        // Holds the loaded translations (key-value pairs)
        private Dictionary<string, string> _translations = new();

        // Current language code (e.g., "en", "fr")
        public string CurrentLanguage { get; private set; } = "en";

        // Path to the config file storing the last used language
        private readonly string configPath = Path.Combine("..", "..", "..", "Model", "Languages", "l_config.json");

        // Load a language file based on the given language code
        public void LoadLanguage(string languageCode)
        {
            string path = Path.Combine("..", "..", "..", "Model", "Languages", $"{languageCode}.json");

            // If the requested language file doesn't exist, fallback to English
            if (!File.Exists(path))
            {
                Console.WriteLine($"Language File not found, fallback to 'en' : ({path}).");
                path = Path.Combine("..", "..", "..", "Model", "Languages", "en.json");
                languageCode = "en";
            }

            // Read and deserialize the translation file
            string json = File.ReadAllText(path);
            _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            CurrentLanguage = languageCode;

            // Save the chosen language to config
            SaveUserLanguage(languageCode);
        }

        // Return the translated string for a given key
        public string Translate(string key)
        {
            return _translations.TryGetValue(key, out var value) ? value : key;
        }

        // Load the last language used by the user from config file
        public void LoadLastUsedLanguage()
        {
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                
                if (config != null && config.TryGetValue("language", out string savedLang))
                {
                    LoadLanguage(savedLang);
                }
            }
            else
            {
                // Default language if no config file exists
                LoadLanguage("en");
            }
        }

        // Save the selected language to the config file
        private void SaveUserLanguage(string languageCode)
        {
            var config = new Dictionary<string, string> { { "language", languageCode } };
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
        }
    }
}

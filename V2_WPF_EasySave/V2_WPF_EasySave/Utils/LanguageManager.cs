using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace V2_WPF_EasySave.Utils
{
    public class LanguageManager : INotifyPropertyChanged
    {
        private static readonly LanguageManager _instance = new();
        public static LanguageManager Instance => _instance;

        private Dictionary<string, string> _translations = new();

        public string this[string key]
        {
            get => _translations.TryGetValue(key, out var value) ? value : $"!{key}!";
        }

        public void LoadLanguage(string langCode)
        {
            string path = Path.Combine("..", "..", "..", "Languages", $"{langCode}.json");
            if (!File.Exists(path)) return;

            string json = File.ReadAllText(path);
            _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            OnPropertyChanged(string.Empty); // notifie toutes les clés
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
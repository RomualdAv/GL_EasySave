using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text.Json.Serialization;

namespace V2_WPF_EasySave.Model
{
    public class JobDef : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public string SourceDirectory { get; set; } = string.Empty;
        public string TargetDirectory { get; set; } = string.Empty;
        public int JobType { get; set; } = 1;

        [JsonIgnore]
        public ManualResetEvent PauseEvent { get; set; } = new ManualResetEvent(true);

        [JsonIgnore]
        public bool StopRequested { get; set; } = false;

        private string _state = "EN_ATTENTE";
        
        [JsonIgnore]
        public string State
        {
            get => _state;
            set { _state = value; OnPropertyChanged(); }
        }

        private int _progression;
        
        [JsonIgnore]
        public int Progression
        {
            get => _progression;
            set { _progression = value; OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

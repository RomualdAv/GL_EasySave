using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text.Json.Serialization;

namespace V2_WPF_EasySave.Model
{
    public class JobDef : INotifyPropertyChanged
    {
        [JsonIgnore]
        public ManualResetEvent PauseEvent { get; set; } = new ManualResetEvent(true);

        private string _state = "EN_ATTENTE";
        private bool _isPaused = false;
        private bool _stopRequested = false;
        private int _progression;

        public string Name { get; set; } = string.Empty;
        public string SourceDirectory { get; set; } = string.Empty;
        public string TargetDirectory { get; set; } = string.Empty;
        public int JobType { get; set; } = 1;

        [JsonIgnore]
        public string State
        {
            get => _state;
            set { _state = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public bool IsPaused
        {
            get => _isPaused;
            set { _isPaused = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public bool StopRequested
        {
            get => _stopRequested;
            set { _stopRequested = value; OnPropertyChanged(); }
        }

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

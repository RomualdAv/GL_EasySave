using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

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


        [JsonIgnore]
        public int Progression { get; set; } = 0;


        [JsonIgnore]
        public string State { get; set; } = "EN_ATTENTE";



        private string _state = "EN_ATTENTE";
        public string State
        {
            get => _state;
            set { _state = value; OnPropertyChanged(); }
        }

        private int _progression;
        public int Progression
        {
            get => _progression;
            set { _progression = value; OnPropertyChanged(); }
        }

        public ManualResetEventSlim PauseEvent { get; set; } = new ManualResetEventSlim(true);


        public bool StopRequested { get; set; } = false;


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

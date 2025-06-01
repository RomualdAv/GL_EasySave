using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using V2_WPF_EasySave.Utils;

namespace V2_WPF_EasySave.ViewModel
{
    public class PriorityExtensionsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Extensions { get; set; }

        private PriorityExtensions _model;

        public ICommand AddExtensionCommand { get; }
        public ICommand RemoveExtensionCommand { get; }
        public ICommand SaveCommand { get; }

        private string _newExtension;
        public string NewExtension
        {
            get => _newExtension;
            set
            {
                _newExtension = value;
                OnPropertyChanged(nameof(NewExtension));
            }
        }

        public PriorityExtensionsViewModel()
        {
            _model = PriorityExtensions.Load();
            Extensions = new ObservableCollection<string>(_model.Extensions);

            AddExtensionCommand = new RelayCommand(_ => AddExtension());
            RemoveExtensionCommand = new RelayCommand(ext => RemoveExtension(ext as string));
            SaveCommand = new RelayCommand(_ => SaveExtensions());
        }

        private void AddExtension()
        {
            if (string.IsNullOrWhiteSpace(NewExtension)) return;

            string ext = NewExtension.Trim().ToLower();
            if (!ext.StartsWith(".")) ext = "." + ext;

            if (!Extensions.Contains(ext))
                Extensions.Add(ext);

            NewExtension = string.Empty;
        }

        private void RemoveExtension(string? ext)
        {
            if (!string.IsNullOrWhiteSpace(ext))
                Extensions.Remove(ext);
        }

        private void SaveExtensions()
        {
            _model.Extensions = Extensions.ToList();
            _model.Save();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

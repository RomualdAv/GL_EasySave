using System.ComponentModel;
using System.Windows.Input;
using V2_WPF_EasySave.Utils;

namespace V2_WPF_EasySave.ViewModel
{
    public class SizeLimitViewModel : INotifyPropertyChanged
    {
        private SizeLimit _model;

        private long _maxSizeKB;
        public long MaxSizeKB
        {
            get => _maxSizeKB;
            set
            {
                if (_maxSizeKB != value)
                {
                    _maxSizeKB = value;
                    OnPropertyChanged(nameof(MaxSizeKB));
                }
            }
        }

        public ICommand SaveCommand { get; }

        public SizeLimitViewModel()
        {
            _model = SizeLimit.Load();
            MaxSizeKB = _model.MaxParallelFileSizeKB;
            SaveCommand = new RelayCommand(_ => Save());
        }

        private void Save()
        {
            if (MaxSizeKB <= 0)
            {
                System.Windows.MessageBox.Show("Veuillez entrer une valeur numérique valide.", "Erreur", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            _model.MaxParallelFileSizeKB = MaxSizeKB;
            _model.Save();
            System.Windows.MessageBox.Show("Paramètre sauvegardé.", "Succès", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
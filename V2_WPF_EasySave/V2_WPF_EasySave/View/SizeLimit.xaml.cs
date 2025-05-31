using System.Windows;
using V2_WPF_EasySave.Utils;

namespace V2_WPF_EasySave.View;

public partial class SizeLimitWindow : Window
{
    private SizeLimit _settings;

    public SizeLimitWindow()
    {
        InitializeComponent();
        _settings = SizeLimit.Load();
        MaxSizeBox.Text = _settings.MaxParallelFileSizeKB.ToString();
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (long.TryParse(MaxSizeBox.Text, out long value) && value > 0)
        {
            _settings.MaxParallelFileSizeKB = value;
            _settings.Save();
            MessageBox.Show("Paramètre sauvegardé.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        else
        {
            MessageBox.Show("Veuillez entrer une valeur numérique valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using V2_WPF_EasySave.Utils;

namespace V2_WPF_EasySave.View;

public partial class PriorityExtensionsWindow : Window
{
    private ObservableCollection<string> _extensions = new();
    private PriorityExtensions _config;

    public PriorityExtensionsWindow()
    {
        InitializeComponent();
        _config = PriorityExtensions.Load();
        _extensions = new ObservableCollection<string>(_config.Extensions);
        ExtensionsListBox.ItemsSource = _extensions;
    }

    private void AddExtension_Click(object sender, RoutedEventArgs e)
    {
        string ext = ExtensionTextBox.Text.Trim().ToLower();
        if (!ext.StartsWith("."))
            ext = "." + ext;

        if (!string.IsNullOrWhiteSpace(ext) && !_extensions.Contains(ext))
            _extensions.Add(ext);

        ExtensionTextBox.Clear();
    }

    private void RemoveExtension_Click(object sender, RoutedEventArgs e)
    {
        if (ExtensionsListBox.SelectedItem is string selected)
            _extensions.Remove(selected);
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        _config.Extensions = _extensions.ToList();
        _config.Save();
        MessageBox.Show("Extensions sauvegardées.");
        this.Close();
    }
}
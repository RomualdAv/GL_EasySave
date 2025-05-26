using System.Windows;
using System.Windows.Controls;
using V2_WPF_EasySave.Utils;
using V2_WPF_EasySave.ViewModel;

namespace V2_WPF_EasySave.View;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        LanguageManager.Instance.LoadLanguage("en");
    }
    
    private void OnLanguageChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string selectedLang = selectedItem.Content.ToString() ?? "fr";
            LanguageManager.Instance.LoadLanguage(selectedLang);
        }
    }
    
    private void JobsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.UpdateSelectedJobs(JobsListBox.SelectedItems.Cast<object>());
        }
    }
}
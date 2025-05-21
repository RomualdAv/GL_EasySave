using System.Windows;
using V2_WPF_EasySave.ViewModel;

namespace V2_WPF_EasySave.View;

public partial class BlockedAppsWindow : Window
{
    public BlockedAppsWindow()
    {
        InitializeComponent();
        DataContext = new BlockedAppsViewModel();
    }
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using V2_WPF_EasySave.Model;
using V2_WPF_EasySave.Utils;

namespace V2_WPF_EasySave.ViewModel;

public class BlockedAppsViewModel : INotifyPropertyChanged
{
    private readonly BlockedAppManager _manager = new();
    public ObservableCollection<string> BlockedApps { get; set; } = new();
    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }

    private string _newApp = string.Empty;
    public string NewApp
    {
        get => _newApp;
        set
        {
            _newApp = value;
            OnPropertyChanged(nameof(NewApp));
        }
    }

    private string? _selectedApp;
    public string? SelectedApp
    {
        get => _selectedApp;
        set
        {
            _selectedApp = value;
            OnPropertyChanged(nameof(SelectedApp));
        }
    }

    public BlockedAppsViewModel()
    {
        foreach (var app in _manager.LoadBlockedApps())
            BlockedApps.Add(app);

        AddCommand = new RelayCommand(_ => AddApp(), _ => !string.IsNullOrWhiteSpace(NewApp));
        RemoveCommand = new RelayCommand(_ => RemoveApp(), _ => SelectedApp != null);
    }

    private void AddApp()
    {
        if (!BlockedApps.Contains(NewApp))
        {
            BlockedApps.Add(NewApp);
            _manager.SaveBlockedApps(BlockedApps.ToList());
            NewApp = string.Empty;
        }
    }

    private void RemoveApp()
    {
        if (SelectedApp != null)
        {
            BlockedApps.Remove(SelectedApp);
            _manager.SaveBlockedApps(BlockedApps.ToList());
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
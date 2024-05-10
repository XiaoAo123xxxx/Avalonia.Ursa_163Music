using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Ursa.Music._163.ViewModels;

public class MenuItemViewModel : ViewModelBase
{
    public MenuItemViewModel()
    {
        ActivateCommand = new AsyncRelayCommand(OnActivate);
    }

    public string MenuHeader { get; set; }
    public string MenuIconName { get; set; }
    public string Key { get; set; }
    public string Status { get; set; }

    public bool IsSeparator { get; set; }
    public ObservableCollection<MenuItemViewModel> Children { get; set; } = new();

    public ICommand ActivateCommand { get; set; }

    private async Task OnActivate()
    {
        await Task.Run(() =>
        {
            if (IsSeparator) return;
            WeakReferenceMessenger.Default.Send<string>(Key);
        });
    }
}
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ursa.Music._163.Models.Messenger;

namespace Ursa.Music._163.Models;

public partial class SearchMusicModel : ObservableObject
{
    [ObservableProperty] private string? _DownloadInfo;
    [ObservableProperty] private string? _FromUrl;
    [ObservableProperty] private int _Id;
    [ObservableProperty] private string? _IncludeAt;
    [ObservableProperty] private string? _MusicPeople;
    [ObservableProperty] private string? _Name;

    [RelayCommand]
    private void ViewPlayMusic(SearchMusicModel searchMusicModel)
    {
        Task.Run(() => { WeakReferenceMessenger.Default.Send(this); });
    }

    private void Downloading()
    {
        Task.Run(() =>
        {
            WeakReferenceMessenger.Default.Send(new WeakMessengerDownloading
            {
                Id = this.Id,
                Name = this.Name,
                MusicPeople = this.MusicPeople,
                IncludeAt = this.IncludeAt,
                FromUrl = this.FromUrl,
                DownloadInfo = this.DownloadInfo,
            });
        });
    }
}
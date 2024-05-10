using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Ursa.Music._163.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private object? _bitmap;
    // public RecommendForMeRouteViewModel RecommendForMeRoute { get; set; } = new RecommendForMeRouteViewModel();

    [ObservableProperty] private object? _content;

    public MainViewModel()
    {
        ChangeContent(this, MenuKey.RecommendForMe);
        WeakReferenceMessenger.Default.Register<MainViewModel, string>(this, ChangeContent);
        // Bitmap = QrCodeHelper.CreateQRCode("https://gitee.com/woshixiaoao1/ursa.-music_163/tree/master", 5, 20);
        Bitmap = "/Assets/QrCode.svg";
    }

    public MenuViewModel Menus { get; set; } = new MenuViewModel();

    public FooterViewModel Footer { get; set; } = new FooterViewModel();

    //public MusicDumpViewModel MusicDump { get; set; } = new MusicDumpViewModel();
    public GlobalConfigViewModel GlobalConfig { get; set; } = new GlobalConfigViewModel();

    public SearchMusicViewModel SearchMusic { get; set; } = new SearchMusicViewModel();

    private void ChangeContent(MainViewModel recipient, string message)
    {
        WeakReferenceMessenger.Default.Send(new MediaColorContent { Status = ColorStatus.Loading });
        Content = null;
        Task.Run(async () =>
        {
            await Task.Delay(500);
            var data = new ViewLocator().GetPageType(message);
            Dispatcher.UIThread.Invoke(() => { Content = (Control)Activator.CreateInstance((Type)data)!; });
        });
    }
}
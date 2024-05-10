using System.Timers;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Ursa.Music._163.ViewModels;

public partial class GlobalConfigViewModel : ViewModelBase
{
    [ObservableProperty] private Color? _FoaBerColor;
    [ObservableProperty] private Color? _FooterColor;
    [ObservableProperty] private Color? _HeaderColor;
    [ObservableProperty] private Color? _NarBerColor;

    public GlobalConfigViewModel()
    {
        HeaderColor = Color.Parse("#4F366C");
        FooterColor = Color.Parse("#13131A");
        NarBerColor = Color.Parse("#4D3767");
        FoaBerColor = Color.Parse("#1A1A21");
        WeakReferenceMessenger.Default.Register<MediaColorContent>(this, ChangeGlobalColor);
    }

    private ColorStatus ColorStatus { get; set; } = ColorStatus.WathetBlue;

    private void ChangeGlobalColor(object recipient, MediaColorContent message)
    {
        var (nav, body) = GetColor(message.Status);

        if (message.Status == ColorStatus.Loading)
        {
            LoadingColor(nav: nav, body: body);
        }
        else
        {
            ColorStatus = message.Status;

            Dispatcher.UIThread.Invoke(() =>
            {
                HeaderColor = Color.Parse(nav);
                NarBerColor = Color.Parse(body);
            });
        }
    }

    private void LoadingColor(string nav, string body)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            HeaderColor = Color.Parse(nav);
            NarBerColor = Color.Parse(body);
        });
        Timer timer = new Timer();
        timer.AutoReset = false;
        timer.Interval = 450;
        timer.Elapsed += (sender, args) =>
        {
            var (navIno, bodyIno) = GetColor(ColorStatus);
            Dispatcher.UIThread.Invoke(() =>
            {
                HeaderColor = Color.Parse(navIno);
                NarBerColor = Color.Parse(bodyIno);
            });
        };
        timer.Start();
    }

    [RelayCommand]
    private void ChangeGlobalColor(string index)
    {
        var intIndex = int.Parse(index);
        var colorStatus = (ColorStatus)intIndex;
        WeakReferenceMessenger.Default.Send(new MediaColorContent { Status = colorStatus });
    }

    private (string nav, string body) GetColor(ColorStatus color)
    {
        return color switch
        {
            ColorStatus.WathetGreen => ("#0D8342", "#16DE70"),
            ColorStatus.WathetBlue => ("#3A456D", "#313B61"),
            ColorStatus.Purple => ("#4F366C", "#4D3767"),
            ColorStatus.Loading => ("#1A1A21", "#13131A"),
            _ => ("#4F366C", "#4D3767"),
        };
    }
}

public class MediaColorContent
{
    public ColorStatus Status { get; set; }
}

public enum ColorStatus
{
    Loading = 0,
    Purple = 1, //紫色
    WathetBlue = 2, //浅蓝色
    WathetGreen = 3, //浅绿色
}
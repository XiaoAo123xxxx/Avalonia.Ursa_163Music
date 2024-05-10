using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using Ursa.Music._163.Utils;
using Ursa.Music._163.Views;

namespace Ursa.Music._163;

public partial class App : Application
{
    private static readonly TrayIcon NotifyIcon = new()
    {
        ToolTipText = "网易云音乐",
        Menu = new NativeMenu()
        {
            new NativeMenuItem()
            {
                Header = "打开程序",
                Command = new RelayCommand(() =>
                {
                    if (MainWindow is not null) MainWindow.IsVisible = true;
                }),
            },
            new NativeMenuItem()
            {
                Header = "退出",
                Command = new RelayCommand(() => { CloseHelper.CloseApplication(); }),
            }
        }
    };

    public static Window? MainWindow { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var bitmap =
            Ursa.Music._163.Converter.PathIcon.LoadFromResource(new Uri("avares://Ursa.Music.163/Assets/Wy163.ico"));
        NotifyIcon.Icon = new WindowIcon(bitmap);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow = new MainWindow();
            desktop.MainWindow = MainWindow;
        }

        // WeakReferenceMessenger.Default.Send<string>(MenuKey.RecommendForMe);
        base.OnFrameworkInitializationCompleted();
    }
}
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using Ursa.Music._163.Models.Messenger;
using Ursa.Music._163.Views;

namespace Ursa.Music._163.Component;

public partial class Header : UserControl
{
    public Header()
    {
        InitializeComponent();
    }

    private void Close_App_Click(object? sender, RoutedEventArgs e)
    {
        var closeWindow = new CloseWindow();

        if (App.MainWindow is not null)
            closeWindow.ShowDialog(App.MainWindow);
    }

    private void MaxSize_App_Click(object? sender, RoutedEventArgs e)
    {
        if (App.MainWindow is null) return;
        if (App.MainWindow.WindowState == WindowState.Maximized)
        {
            App.MainWindow.WindowState = WindowState.Normal;
            return;
        }

        App.MainWindow.WindowState = WindowState.Maximized;
    }

    private void MinSize_App_Click(object? sender, RoutedEventArgs e)
    {
        if (App.MainWindow is not null)
            App.MainWindow.WindowState = WindowState.Minimized;
    }

    private void SearchTextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            WeakReferenceMessenger.Default.Send(new WeakMessengerSearchMusicContent()
            {
                SearchMusicName = SearchTextBox.Text?.Replace("\n", ""),
            });
        }
    }
}
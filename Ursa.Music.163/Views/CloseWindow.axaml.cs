using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Ursa.Music._163.Utils;

namespace Ursa.Music._163.Views;

public partial class CloseWindow : Window
{
    public CloseWindow()
    {
        InitializeComponent();
        this.PointerPressed += CloseWindow_PointerPressed;
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Border.CornerRadius = new CornerRadius(0);
        }
    }

    private void CloseWindow_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse)
        {
            this.BeginMoveDrag(e);
        }
    }

    private void Close_This_Window_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void Yes_Button_Click(object? sender, RoutedEventArgs e)
    {
        if (Close_App.IsChecked != null && (bool)Close_App.IsChecked)
        {
            CloseHelper.CloseApplication();
        }
        else
        {
            if (App.MainWindow is not null)
                App.MainWindow.IsVisible = false;
        }
    }
}
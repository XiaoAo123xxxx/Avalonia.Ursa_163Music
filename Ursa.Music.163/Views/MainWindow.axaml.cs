using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Ursa.Music._163.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.PointerPressed += MainWindow_PointerPressed;
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Border.CornerRadius = new CornerRadius(0);
        }
    }

    private void MainWindow_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse)
        {
            this.BeginMoveDrag(e);
        }
    }
}
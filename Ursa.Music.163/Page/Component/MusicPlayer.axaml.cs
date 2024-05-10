using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Ursa.Music._163.Utils;

namespace Ursa.Music._163.Page.Component;

public partial class MusicPlayer : UserControl
{
    private Color[] _colors = ColorUtils.GetAllHsvColors();
    private Random _random = new Random();
    WasapiCapture? capture;
    private List<double>? spectrumData;
    private double[]? spectrumDataService;

    Visualizer? visualizer;

    public MusicPlayer()
    {
        InitializeComponent();

        capture = new WasapiLoopbackCapture(); // 捕获电脑发出的声音
        visualizer = new Visualizer(256);
        capture.WaveFormat =
            WaveFormat.CreateIeeeFloatWaveFormat(8192, 1); // 指定捕获的格式, 单声道, 32位深度, IeeeFloat 编码, 8192采样率
        capture.DataAvailable += Capture_DataAvailable;
        capture.StartRecording();
        Timer timer = new Timer(); //采集
        timer.Interval = 30;
        timer.Elapsed += (sender, args) => { LockMusic(); };
        timer.Start();

        Timer showTimer = new Timer();
        showTimer.Interval = 100;
        showTimer.Elapsed += ShowData;
        showTimer.Start();
    }

    private void ShowData(object? sender, ElapsedEventArgs e)
    {
        if (spectrumDataService == null) return;

        DrwaingContent(MusicCanvas, spectrumDataService);
    }

    private void LockMusic()
    {
        double[] newSpectrumData = visualizer.GetSpectrumData(); // 从可视化器中获取频谱数据
        newSpectrumData = Visualizer.GetBlurry(newSpectrumData, 2);
        spectrumDataService = newSpectrumData;
    }

    private void Capture_DataAvailable(object? sender, WaveInEventArgs e)
    {
        int length = e.BytesRecorded / 4; // 采样的数量 (每一个采样是 4 字节)
        double[] result = new double[length]; // 声明结果

        for (int i = 0; i < length; i++)
            result[i] = BitConverter.ToSingle(e.Buffer, i * 4); // 取出采样值

        visualizer.PushSampleData(result); // 将新的采样存储到 可视化器 中
    }

    private async void DrwaingContent(StackPanel c, IEnumerable<double>? height = null)
    {
        if (height is null) return;

        var heightList = height.ToList();

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            c.Children.Clear();
            for (int i = 0; i < heightList.Count(); i++)
            {
                LinearGradientBrush brush = new LinearGradientBrush();
                brush.StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative);
                brush.EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative);
                var color1 = _colors[_random.Next(0, _colors.Length)];
                var color2 = _colors[_random.Next(0, _colors.Length)];
                brush.GradientStops.Add(new GradientStop(color1, 0));
                brush.GradientStops.Add(new GradientStop(color2, 1));
                Rectangle rectangle = new();
                rectangle.RadiusX = 5;
                rectangle.RadiusY = 5;
                rectangle.Fill = brush;
                rectangle.Width = 10;
                rectangle.Height = heightList[i] * 200000;
                rectangle.Margin = new Thickness(1);
                rectangle.VerticalAlignment = VerticalAlignment.Bottom;
                c.Children.Add(rectangle);
            }
        });
    }
}
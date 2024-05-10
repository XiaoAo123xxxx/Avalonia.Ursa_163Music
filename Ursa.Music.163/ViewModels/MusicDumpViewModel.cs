using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Timers;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Ursa.Music._163.Utils;
using Timer = System.Timers.Timer;

namespace Ursa.Music._163.ViewModels;

public partial class MusicDumpViewModel : ViewModelBase
{
    /// <summary>
    /// 采样数据的对象锁，防止未分离左右通道就进入下一次采样
    /// </summary>
    private object _sampleLock = new object();

    /// <summary>
    /// 频域数据
    /// </summary>
    [ObservableProperty] private double[]? _Value;

    WasapiCapture? capture;
    [ObservableProperty] private ObservableCollection<double>? spectrumData = new ObservableCollection<double>();
    private double[]? spectrumDataService;

    Visualizer? visualizer;

    public MusicDumpViewModel()
    {
        capture = new WasapiLoopbackCapture(); // 捕获电脑发出的声音
        visualizer = new Visualizer(256);
        capture.WaveFormat =
            WaveFormat.CreateIeeeFloatWaveFormat(8192, 1); // 指定捕获的格式, 单声道, 32位深度, IeeeFloat 编码, 8192采样率
        capture.DataAvailable += Capture_DataAvailable;
        capture.StartRecording();
        Timer timer = new Timer(); //采集
        timer.Interval = 200;
        timer.Elapsed += (sender, args) => { LockMusic(); };
        timer.Start();
        SpectrumData = new ObservableCollection<double>();
        for (int i = 0; i < 8; i++)
        {
            SpectrumData.Add(0);
        }

        // Task.Factory.StartNew(ShowDataToView, TaskCreationOptions.LongRunning);
        Timer showTimer = new Timer();
        showTimer.Interval = 55;
        showTimer.Elapsed += ShowData;
        showTimer.Start();
    }

    private void ShowData(object? sender, ElapsedEventArgs e)
    {
        if (spectrumDataService == null) return;

        Dispatcher.UIThread.Invoke(() =>
        {
            try
            {
                for (int i = 0; i * 16 < spectrumDataService.Length; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < 16; j++)
                    {
                        sum += spectrumDataService[i + j];
                    }

                    var height = sum / 16;
                    SpectrumData[i] = height > 99 ? 100 : height;
                }
            }
            catch (Exception)
            {
            }
        });
    }

    private void ShowDataToView()
    {
        while (true)
        {
            for (int i = 0; i < spectrumDataService.Length; i++)
            {
                SpectrumData[i] = spectrumDataService[i];
            }

            Thread.Sleep(100);
        }
    }

    private void Capture_DataAvailable(object? sender, WaveInEventArgs e)
    {
        int length = e.BytesRecorded / 4; // 采样的数量 (每一个采样是 4 字节)
        double[] result = new double[length]; // 声明结果

        for (int i = 0; i < length; i++)
            result[i] = BitConverter.ToSingle(e.Buffer, i * 4); // 取出采样值

        visualizer.PushSampleData(result); // 将新的采样存储到 可视化器 中
    }

    private void LockMusic()
    {
        double[] newSpectrumData = visualizer.GetSpectrumData(); // 从可视化器中获取频谱数据
        newSpectrumData = Visualizer.GetBlurry(newSpectrumData, 2);

        spectrumDataService = newSpectrumData;


        // double[] bassArea = Visualizer.TakeSpectrumOfFrequency(SpectrumData, capture.WaveFormat.SampleRate, 250);  
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ursa.Controls;
using Ursa.Music._163.Models.Messenger;
using Ursa.Music._163.Utils;

namespace Ursa.Music._163.ViewModels;

public partial class FooterViewModel : ViewModelBase
{
    private readonly List<PlayerMode>? _allModes;

    readonly Dictionary<string, object?> musics = new Dictionary<string, object?>();
    [ObservableProperty] private string _AllTime;

    private bool _isLike;

    private bool _isPlayer;

    [ObservableProperty]
    private ObservableCollection<ViewShowSource> _LoverMusic = new ObservableCollection<ViewShowSource>();

    [ObservableProperty] private double _MaxPlan;

    [ObservableProperty] private double _MinPlan;

    [ObservableProperty] private List<string> _MusicPathGather = new List<string>();
    [ObservableProperty] private double _NowPlan;

    [ObservableProperty] private string? _NowPlayerMusicName;

    [ObservableProperty] private string _NowTime;
    [ObservableProperty] private PlayerMode? _playerMode;

    [ObservableProperty] private string? _playLike;
    [ObservableProperty] private string? _playStatus;

    private string? _SelectPlay = string.Empty;

    private double _volume;

    private bool Error = false;

    private string musicPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music", "就是南方凯 - 离别开出花.mp3");

    public FooterViewModel()
    {
        _allModes = new List<PlayerMode>()
        {
            new() { ModeContent = "心动模式", ModeIcon = "avares://Ursa.Music.163/Assets/Footer/心动.png" },
            new() { ModeContent = "随机播放", ModeIcon = "avares://Ursa.Music.163/Assets/Footer/随机播放.png" },
            new() { ModeContent = "顺序播放", ModeIcon = "avares://Ursa.Music.163/Assets/Footer/顺序播放.png" },
            new() { ModeContent = "单曲循环", ModeIcon = "avares://Ursa.Music.163/Assets/Footer/单曲循环.png" },
        };

        PlayerMode = _allModes[0];
        ChangeModeCommand = new RelayCommand(ChangeMode);
        ChangeLikeCommand = new RelayCommand(ChangeLike);
        ChangePlayCommand = new RelayCommand(ChangePlay);
        IsLike = false;
        IsPlayer = false;
        Init();
    }

    public ICommand ChangeModeCommand { get; set; }
    public ICommand ChangeLikeCommand { get; set; }
    public ICommand ChangePlayCommand { get; set; }

    public bool IsLike
    {
        get => _isLike;
        set
        {
            PlayLike = value
                ? "avares://Ursa.Music.163/Assets/Footer/喜欢1.png"
                : "avares://Ursa.Music.163/Assets/Footer/喜欢2.png";
            SetProperty(ref _isLike, value);
        }
    }

    public bool IsPlayer
    {
        get => _isPlayer;
        set
        {
            PlayStatus = value
                ? "avares://Ursa.Music.163/Assets/Footer/暂停.png"
                : "avares://Ursa.Music.163/Assets/Footer/播放.png";
            SetProperty(ref _isPlayer, value);
        }
    }

    public double Volume
    {
        get => _volume;
        set =>
            SetProperty(ref _volume, (int)value);
    }

    private void ChangePlay()
    {
        IsPlayer = !IsPlayer;
        //停止播放音乐
        if (!IsPlayer)
        {
            if (musics.TryGetValue(musicPath, out object playerObjStop))
            {
                MciPlayer player = (MciPlayer)playerObjStop;
                player.Pause();
            }

            return;
        }

        //播放音乐
        if (musics.TryGetValue(musicPath, out object playerObjPlay))
        {
            MciPlayer player = (MciPlayer)playerObjPlay;
            player.Play();
            ModeReset();
        }
    }

    private void ModeReset()
    {
        // 播放模式
        if (PlayerMode == _allModes[3])
        {
            musics.TryGetValue(musicPath, out object playerObjPlay);
            MciPlayer player = (MciPlayer)playerObjPlay;
            player.PlayRepeat();
        }
    }

    private void Init()
    {
        // 初始化
        LoadingMusicGather();
        MciPlayer? player = new MciPlayer(musicPath);
        player.Open();
        musics[musicPath] = player;
        LoadingMusicName();
        System.Timers.Timer timer = new System.Timers.Timer();
        timer.Interval = 1000;
        timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        timer.Start();
        WeakReferenceMessenger.Default.Register<PlayerNowContent>(this, ChangeNowContent);
        WeakReferenceMessenger.Default.Register<WeakMessengerTryPlayWebMusic>(this, PlayWebUrlMusic);
    }

    private void PlayWebUrlMusic(object recipient, WeakMessengerTryPlayWebMusic message)
    {
        var index = LoverMusic.Count;
        var loverHaving = LoverMusic.FirstOrDefault(e => e.Content == message.PlayName);
        if (loverHaving is null)
        {
            MusicPathGather.Add(message.FileName);
            LoverMusic.Add(new ViewShowSource
            {
                Path = message.FileName,
                Index = index,
                Content = message.PlayName,
                IsPlay = false
            });
        }
        //
        // Dispatcher.UIThread.Invoke(() =>
        // {
        //     ChangeNowContent(this, new PlayerNowContent
        //     {
        //         Content = message.PlayName
        //     });
        // });
    }

    private void ChangeNowContent(object recipient, PlayerNowContent message)
    {
        //停止当前
        musics.TryGetValue(musicPath, out object playerObjStop);
        MciPlayer playerClose = (MciPlayer)playerObjStop;
        if (playerClose is not null)
        {
            playerClose.Pause();
            playerClose.Dispose();
        }


        //查询路径
        var file = LoverMusic.FirstOrDefault(e => e.Content == message.Content);
        if (file is null)
            return;
        musicPath = file.Path;
        MciPlayer? player = new MciPlayer(musicPath);
        // try
        // {
        player.Open();
        // }
        // catch (Exception e)
        // {
        //     ChangeNowContent(this, message);
        // }

        // player.Open();
        musics[musicPath] = player;
        //播放
        musics.TryGetValue(musicPath, out object playerObjPlay);
        MciPlayer playerNow = (MciPlayer)playerObjPlay;
        playerNow.Play();
        ModeReset();
        LoadingMusicName();
        Error = false;
        IsPlayer = true;
    }

    [RelayCommand]
    private void PlayerAllMusic()
    {
        if (LoverMusic.Count > 0)
            WeakReferenceMessenger.Default.Send(new PlayerNowContent { Content = LoverMusic[0].Content });
        PlayerMode = _allModes[2];
    }


    private void ChangeContentColor(string? message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        if (!string.IsNullOrWhiteSpace(_SelectPlay))
        {
            var data = LoverMusic.FirstOrDefault(e => e.Content == _SelectPlay);
            if (data is not null)
                data.IsPlay = false;
        }

        var info = LoverMusic.FirstOrDefault(e => e.Content == message);
        if (info == null) return;
        info.IsPlay = true;
        _SelectPlay = message;
    }


    private void LoadingMusicName()
    {
        // 加载名字
        var nowName = musicPath.Split(@"\");
        var endName = nowName[^1].Split(".");
        NowPlayerMusicName = endName[0];
        ChangeContentColor(NowPlayerMusicName);
    }

    private string LoadingMusicName(string name)
    {
        var nowName = name.Split(@"\");
        var endName = nowName[^1].Split(".");
        return endName[0];
    }

    private void LoadingMusicGather()
    {
        //加载播放列表集合
        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");
        string[] files = Directory.GetFiles(dir);
        MusicPathGather.Clear();
        LoverMusic.Clear();
        int index = 1;
        foreach (var file in files)
        {
            if (file.EndsWith(".mp3"))
            {
                MusicPathGather.Add(file);
                var name = LoadingMusicName(file);
                LoverMusic.Add(new ViewShowSource
                {
                    Index = index++, Content = name, IsPlay = false, Path = file,
                });
            }

            if (file.EndsWith(".m4a"))
            {
                File.Delete(file);
            }
        }
    }


    [RelayCommand]
    private void PreviousSong()
    {
        //上一曲
        for (var i = 0; i < MusicPathGather.Count; i++)
        {
            if (MusicPathGather[i] == musicPath)
            {
                try
                {
                    //暂停
                    musics.TryGetValue(musicPath, out object? playerObjStop);
                    MciPlayer? playerClose = (MciPlayer)playerObjStop;
                    playerClose?.Pause();
                    playerClose?.Dispose();
                    musicPath = i == 0 ? MusicPathGather[^1] : MusicPathGather[i - 1];
                    if (PlayerMode == _allModes?[1])
                    {
                        Random r = new Random();
                        var index = r.Next(1, 10);
                        musicPath = (i + index) <= MusicPathGather.Count - 1
                            ? MusicPathGather[i + index]
                            : (i - index) >= 0
                                ? MusicPathGather[i - index]
                                : (i + index / 2) <= MusicPathGather.Count - 1
                                    ? MusicPathGather[i + index / 2]
                                    : MusicPathGather[0];
                    }

                    // musics.Clear();
                    //加载
                    MciPlayer? player = new MciPlayer(musicPath);
                    player.Open();
                    musics[musicPath] = player;
                    //播放
                    musics.TryGetValue(musicPath, out object playerObjPlay);
                    MciPlayer playerNow = (MciPlayer)playerObjPlay;
                    playerNow.Play();
                    ModeReset();
                    LoadingMusicName();
                    IsPlayer = true;
                    Error = false;
                }
                catch (Exception e)
                {
                    Error = true;
                    LoadingMusicName();
                    MessageBox.ShowAsync("加载失败");
                }

                return;
            }
        }
    }

    [RelayCommand]
    private void NextSong()
    {
        //下一曲
        for (var i = 0; i < MusicPathGather.Count; i++)
        {
            if (MusicPathGather[i] == musicPath)
            {
                try
                {
                    //暂停
                    musics.TryGetValue(musicPath, out object playerObjStop);
                    MciPlayer playerClose = (MciPlayer)playerObjStop;
                    playerClose.Pause();
                    playerClose.Dispose();
                    musicPath = i == MusicPathGather.Count - 1 ? MusicPathGather[0] : MusicPathGather[i + 1];
                    if (PlayerMode == _allModes?[1])
                    {
                        Random r = new Random();
                        var index = r.Next(1, 10);
                        musicPath = (i + index) <= MusicPathGather.Count - 1
                            ? MusicPathGather[i + index]
                            : (i - index) >= 0
                                ? MusicPathGather[i - index]
                                : (i + index / 2) <= MusicPathGather.Count - 1
                                    ? MusicPathGather[i + index / 2]
                                    : MusicPathGather[0];
                    }

                    // musics.Clear();
                    //加载
                    MciPlayer? player = new MciPlayer(musicPath);
                    player.Open();
                    musics[musicPath] = player;
                    //播放
                    musics.TryGetValue(musicPath, out object playerObjPlay);
                    MciPlayer playerNow = (MciPlayer)playerObjPlay;
                    playerNow.Play();
                    ModeReset();
                    LoadingMusicName();
                    Error = false;
                    IsPlayer = true;
                }
                catch (Exception e)
                {
                    Error = true;
                    LoadingMusicName();
                    MessageBox.ShowAsync("加载失败");
                }

                return;
            }
        }
    }

    private void timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        // 刷新 播放进度
        if (Error) return;
        Dispatcher.UIThread.Invoke(() =>
        {
            try
            {
                musics.TryGetValue(musicPath, out object playerObjPlay);
                MciPlayer player = (MciPlayer)playerObjPlay;
                var length = player.GetLength();
                var position = player.GetPosition();
                MaxPlan = length;
                NowPlan = position;
                NowTime = IntStringToTimeString((position / 340));
                AllTime = IntStringToTimeString((length / 340));
                if (length == position && PlayerMode != _allModes?[3])
                {
                    NextSong();
                }
            }
            catch (Exception)
            {
            }
        });
    }

    private string IntStringToTimeString(int secondGlo)
    {
        // 解析歌曲时间
        var second = secondGlo / 3;
        var min = second / 60;
        var se = second % 60;
        if (se < 10)
            return $"{min}:0{se}";
        return $"{min}:{se}";
    }

    private void ChangeLike()
    {
        IsLike = !IsLike;
    }

    private void ChangeMode()
    {
        // 修改模式
        if (_allModes is null) return;
        for (var i = 0; i < _allModes.Count; i++)
        {
            if (_allModes[i].ModeContent != PlayerMode?.ModeContent) continue;
            PlayerMode = i == _allModes.Count - 1 ? _allModes[0] : _allModes[i + 1];
            ModeReset();
            return;
        }
    }

    [RelayCommand]
    private void MoveMusicLength()
    {
        // 拖动播放
        musics.TryGetValue(musicPath, out object playerObj);
        MciPlayer player = (MciPlayer)playerObj;
        player.Seek((int)NowPlan);
        player.Play();
        ModeReset();
        IsPlayer = true;
    }
}

public partial class PlayerMode : ViewModelBase
{
    [ObservableProperty] private string? _modeContent;
    [ObservableProperty] private string? _modeIcon;
}

public partial class ViewShowSource : ViewModelBase
{
    [ObservableProperty] private string? _Content;
    [ObservableProperty] private int _Index;
    [ObservableProperty] private bool _IsPlay;
    public string? Path { get; set; }

    [RelayCommand]
    private void ChangeMusic(string name)
    {
        WeakReferenceMessenger.Default.Send(new PlayerNowContent { Content = name });
    }
}

public class PlayerNowContent
{
    public string? Content { get; set; }
}
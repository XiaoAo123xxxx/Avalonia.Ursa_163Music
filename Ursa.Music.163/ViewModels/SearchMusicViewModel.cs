using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Ursa.Controls;
using Ursa.Music._163.Models;
using Ursa.Music._163.Models.Messenger;
using Ursa.Music._163.Utils;

namespace Ursa.Music._163.ViewModels;

public partial class SearchMusicViewModel : ViewModelBase
{
    [ObservableProperty] private string? _searchContent;
    [ObservableProperty] private ObservableCollection<SearchMusicModel>? _searchMusicCollection = new();

    private bool IsUsingWebClient = false;


    private string TempFileLoad = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");

    public SearchMusicViewModel()
    {
        WeakReferenceMessenger.Default.Register<WeakMessengerSearchMusicContent>(this, SearchMusic);
        WeakReferenceMessenger.Default.Register<SearchMusicModel>(this, SearchMusicReference);
    }

    private void SearchMusicReference(object recipient, SearchMusicModel message)
    {
        PlayUrlMusic(message);
    }

    private void SearchMusic(object recipient, WeakMessengerSearchMusicContent message)
    {
        SearchMusic(message.SearchMusicName);
    }

    private void SearchMusic(string? searchMusicName)
    {
        if (string.IsNullOrWhiteSpace(searchMusicName)) return;
        string strName = searchMusicName;
        MusicOperation mop = new MusicOperation();
        List<clsMusic> lmsc = new List<clsMusic>();

        Task.Run(async () =>
        {
            try
            {
                lmsc = await mop.GetMusicList(strName);
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    SearchMusicCollection = new ObservableCollection<SearchMusicModel>();
                    for (int i = 0; i < lmsc.Count; i++)
                    {
                        SearchMusicCollection.Add(new SearchMusicModel()
                        {
                            Id = i + 1,
                            Name = lmsc[i].Name + (lmsc[i].Subheading == "" ? "" : lmsc[i].Subheading),
                            MusicPeople = lmsc[i].Singer,
                            IncludeAt = lmsc[i].Class,
                            FromUrl = MusicSourceHelper.GetSourceUrl(lmsc[i].Source),
                            DownloadInfo = lmsc[i].DownloadInfo,
                        });
                    }

                    WeakReferenceMessenger.Default.Send<string>(MenuKey.SearchMusic);
                });
            }
            catch (Exception e)
            {
            }
        });
    }

    private async void PlayUrlMusic(SearchMusicModel searchMusic)
    {
        if (IsUsingWebClient)
        {
            Dispatcher.UIThread.InvokeAsync(() => { MessageBox.ShowAsync("稍等哦！上一个还没有爬取完成~"); });
            return;
        }

        if (searchMusic?.FromUrl is null) return;
        IsUsingWebClient = true;
        enmMusicSource emsSource = (enmMusicSource)MusicSourceHelper.GetSourceInt(searchMusic.FromUrl!);
        string strDownloadInfo = searchMusic.DownloadInfo!;
        string strDisplayName = searchMusic.Name!;
        string strDisplayId = searchMusic.Id.ToString();

        MusicOperation mop = new MusicOperation();
        clsHttpDownloadFile hdf = new clsHttpDownloadFile();

        try
        {
            if (!Directory.Exists(TempFileLoad)) //如果不存在就创建 临时文件夹  
                Directory.CreateDirectory(TempFileLoad);
            var strDownloadURL = await mop.GetMusicDownloadURL(strDownloadInfo, emsSource);
            var endFileName = mop.GetFileFormat();
            var strFileName =
                TempFileLoad + "\\" + strDisplayName + "_" + searchMusic.MusicPeople +
                endFileName; //临时文件夹 + dgvDownloadInfo + 格式
            var mbFileName = TempFileLoad + "\\" + strDisplayName + "_" + searchMusic.MusicPeople + ".mp3";

            if (!(File.Exists(mbFileName))) //不存在缓存，才下载
            {
                try
                {
                    if (strDownloadURL == "")
                    {
                        IsUsingWebClient = false;
                        Dispatcher.UIThread.InvokeAsync(() => { MessageBox.ShowAsync("爬取失败:" + "格式校验失败"); });
                        return;
                    }

                    if (!hdf.Download(strDownloadURL, strFileName))
                    {
                        if (!File.Exists(strFileName))
                        {
                            IsUsingWebClient = false;
                            Dispatcher.UIThread.InvokeAsync(() => { MessageBox.ShowAsync("爬取失败:" + "VIP歌曲无法获取"); });
                            return;
                        }
                    }

                    if (endFileName.Contains("m4a"))
                    {
                        var savePath = TempFileLoad + "\\" + strDisplayName + "_" + searchMusic.MusicPeople + ".mp3";
                        var result = ffmpegHelper.ConvertAudio(strFileName,
                            savePath);
                        if (!result.Contains("格式转换成功"))
                        {
                            Dispatcher.UIThread.InvokeAsync(() => { MessageBox.ShowAsync(result); });
                            IsUsingWebClient = false;
                            return;
                        }

                        WeakReferenceMessenger.Default.Send(new WeakMessengerTryPlayWebMusic
                        {
                            FileName = savePath, PlayName = strDisplayName + "_" + searchMusic.MusicPeople,
                        });
                        IsUsingWebClient = false;
                        return;
                    }

                    WeakReferenceMessenger.Default.Send(new WeakMessengerTryPlayWebMusic
                    {
                        FileName = strFileName, PlayName = strDisplayName + "_" + searchMusic.MusicPeople,
                    });
                }
                catch (Exception e)
                {
                    Dispatcher.UIThread.InvokeAsync(() => { MessageBox.ShowAsync("Error:" + e.Message); });
                }
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new WeakMessengerTryPlayWebMusic
                {
                    FileName = mbFileName, PlayName = strDisplayName + "_" + searchMusic.MusicPeople,
                });
                // Dispatcher.UIThread.InvokeAsync(() => { MessageBox.ShowAsync("已经下载过来哦"); });
            }

            IsUsingWebClient = false;
        }
        catch (Exception e)
        {
            IsUsingWebClient = false;
        }
    }
}
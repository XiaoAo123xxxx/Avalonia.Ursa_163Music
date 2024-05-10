using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ursa.Controls;

namespace Ursa.Music._163.ViewModels;

public partial class RecommendForMeRouteViewModel : ViewModelBase
{
    [ObservableProperty] private object? _sceneBitMap = "avares://Ursa.Music.163/Assets/Scene/夜晚.jpg";
    [ObservableProperty] private string? _sceneHeader;

    public RecommendForMeRouteViewModel()
    {
        // ChangeScene("0");
    }

    [RelayCommand]
    private void RoutePush(string route)
    {
        MessageBox.ShowAsync(route);
    }

    [RelayCommand]
    private void ChangeScene(string scene)
    {
        var sceneEnum = (Scene)int.Parse(scene);
        (SceneBitMap, SceneHeader) = sceneEnum switch
        {
            Scene.Night => ("avares://Ursa.Music.163/Assets/Scene/夜晚.jpg", "夜晚"),
            Scene.Study => ("avares://Ursa.Music.163/Assets/Scene/学习.jpg", "学习"),
            Scene.Work => ("avares://Ursa.Music.163/Assets/Scene/工作.jpg", "工作"),
            Scene.OnFoot => ("avares://Ursa.Music.163/Assets/Scene/散步.jpg", "散步"),
            Scene.Tour => ("avares://Ursa.Music.163/Assets/Scene/旅行.jpg", "旅行"),
            Scene.Live => ("avares://Ursa.Music.163/Assets/Scene/生活.jpg", "生活"),
            _ => ("avares://Ursa.Music.163/Assets/Scene/夜晚.jpg", "夜晚"),
        };
    }
}

public enum Scene
{
    Night = 0,
    Study = 1,
    Work = 2,
    OnFoot = 3,
    Tour = 4,
    Live = 5
}
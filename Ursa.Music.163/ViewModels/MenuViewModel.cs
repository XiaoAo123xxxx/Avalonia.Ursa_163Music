using System.Collections.ObjectModel;

namespace Ursa.Music._163.ViewModels;

public class MenuViewModel : ViewModelBase
{
    public ObservableCollection<MenuItemViewModel>? MenuItems { get; set; } = new()
    {
        new() { MenuHeader = "为我推荐", Key = MenuKey.RecommendForMe, },
        new() { MenuHeader = "云音乐精选", Key = MenuKey.CloudMusicCollection, },
        new() { MenuHeader = "博客", Key = MenuKey.Blog, },
        new() { MenuHeader = "私人漫游", Key = MenuKey.PrivateRoaming, },
        new() { MenuHeader = "社区", Key = MenuKey.Community, },
        new() { MenuHeader = "", Key = "", IsSeparator = true },
        new() { MenuHeader = "我喜欢的音乐", Key = MenuKey.LoverContent, },
        new() { MenuHeader = "最近播放", Key = MenuKey.RecentlyPlayed, },
        new() { MenuHeader = "我的博客", Key = MenuKey.MyBlog, },
        new() { MenuHeader = "我的收藏", Key = MenuKey.MyCollection, },
        new() { MenuHeader = "下载管理", Key = MenuKey.DownLoad, },
        new() { MenuHeader = "本地音乐", Key = MenuKey.LocalMusic, },
        new() { MenuHeader = "我的音乐云盘", Key = MenuKey.MyMusicCloud, },
    };
}

public static class MenuKey
{
    public const string RecommendForMe = "RecommendForMe";
    public const string CloudMusicCollection = "CloudMusicCollection";
    public const string Blog = "Blog";
    public const string PrivateRoaming = "PrivateRoaming";
    public const string Community = "Community";
    public const string LoverContent = "LoverContent";
    public const string RecentlyPlayed = "RecentlyPlayed";
    public const string MyBlog = "MyBlog";
    public const string MyCollection = "MyCollection";
    public const string DownLoad = "DownLoad";
    public const string LocalMusic = "LocalMusic";
    public const string MyMusicCloud = "MyMusicCloud";
    public const string SearchMusic = "SearchMusic";
}
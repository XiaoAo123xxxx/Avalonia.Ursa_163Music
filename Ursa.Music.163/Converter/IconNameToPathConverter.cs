using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Ursa.Music._163.Converter;

public class IconNameToPathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i)
        {
        }
        else if (value is string s)
        {
            var path = PathIcon.Icons.FirstOrDefault(e => e.Item1 == (string)value).Item2;
            if (path is null)
                return AvaloniaProperty.UnsetValue;
            return PathIcon.LoadFromResource(new Uri(path));
            ;
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}

public static class PathIcon
{
    public static (string, string)[] Icons { get; set; } = new (string, string)[]
    {
        ("为我推荐", "avares://Ursa.Music.163/Assets/Navicat/tj.png"),
        ("云音乐精选", "avares://Ursa.Music.163/Assets/Navicat/jx.png"),
        ("博客", "avares://Ursa.Music.163/Assets/Navicat/bk.png"),
        ("私人漫游", "avares://Ursa.Music.163/Assets/Navicat/my.png"),
        ("社区", "avares://Ursa.Music.163/Assets/Navicat/sq.png"),

        ("我喜欢的音乐", "avares://Ursa.Music.163/Assets/Navicat/我喜欢.png"),
        ("最近播放", "avares://Ursa.Music.163/Assets/Navicat/最近播放.png"),
        ("我的博客", "avares://Ursa.Music.163/Assets/Navicat/博客.png"),
        ("我的收藏", "avares://Ursa.Music.163/Assets/Navicat/收藏.png"),
        ("下载管理", "avares://Ursa.Music.163/Assets/Navicat/下载.png"),
        ("本地音乐", "avares://Ursa.Music.163/Assets/Navicat/本地音乐.png"),
        ("我的音乐云盘", "avares://Ursa.Music.163/Assets/Navicat/云盘.png"),
    };

    public static Bitmap LoadFromResource(Uri resourceUri)
    {
        return new Bitmap(AssetLoader.Open(resourceUri));
    }

    public static async Task<Bitmap?> LoadFromWeb(Uri url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
    }
}
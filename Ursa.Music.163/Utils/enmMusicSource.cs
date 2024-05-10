namespace Ursa.Music._163.Utils;

/// <summary>
/// 来源，0-，1-QQ音乐，2-酷狗音乐
/// </summary>
public enum enmMusicSource
{
    Nothing = 0,
    QQ = 1,
    Kg = 2,
    Kw = 3,
    Wyy = 4
}

public static class MusicSourceHelper
{
    public static string GetSourceUrl(enmMusicSource source)
    {
        return source switch
        {
            enmMusicSource.Nothing => "Error",
            enmMusicSource.QQ => "QQ音乐",
            enmMusicSource.Kg => "酷狗音乐",
            enmMusicSource.Kw => "酷我音乐",
            enmMusicSource.Wyy => "网易云音乐"
        };
    }

    public static int GetSourceInt(string enmMusicSource)
    {
        return enmMusicSource switch
        {
            "Error" => 0,
            "QQ音乐" => 1,
            "酷狗音乐" => 2,
            "酷我音乐" => 3,
            "网易云音乐" => 4,
            _ => 0
        };
    }
}
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Ursa.Music._163.Utils;

public class MciPlayer : IDisposable
{
    private string aliasName;

    private string longpath;
    private string shortName;

    public MciPlayer()
    {
    }

    public MciPlayer(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("File not exist.", path);

        longpath = path;
    }


    public string DevicePath
    {
        get => longpath;
    }

    public string DeviceShortPath
    {
        get => shortName;
    }

    public string AliasName
    {
        get => aliasName;
    }

    public void Dispose()
    {
        if (aliasName != null)
            Close();
    }

    [DllImport("kernel32.dll", EntryPoint = "GetShortPathNameW", CharSet = CharSet.Unicode)]
    extern static short GetShortPath(string longPath, string buffer, int bufferSize);

    [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Unicode)]
    extern static int MciSendString(string command, string buffer, int bufferSize, IntPtr callback);

    bool TryGetShortPath(string longPath, out string shortPath)
    {
        shortPath = null;
        short reqLen = GetShortPath(longPath, null, 0); // 指定 null与0, 则返回需要的长度
        if (reqLen == 0)
            return false;

        shortPath = new string('\0', reqLen); // 声明缓冲

        short rstLen = GetShortPath(longPath, shortPath, reqLen); // 转换
        if (rstLen == 0 || rstLen == reqLen)
            return false;

        shortPath = shortPath.TrimEnd('\0');
        return true;
    }

    void MciSendStringWithCheck(string command, string buffer, int bufferSize, IntPtr callback)
    {
        int err = MciSendString(command, buffer, bufferSize, callback);
        if (err != 0)
            throw new MciException(err);
    }

    string StatusInfo(string info)
    {
        string buffer = new string('\0', 32);
        MciSendStringWithCheck($"status {aliasName} {info}", buffer, 32, IntPtr.Zero);

        return buffer.TrimEnd('\0');
    }

    public bool SetDevicePath(string longpath)
    {
        if (aliasName != null)
            return false;

        this.longpath = longpath;
        return true;
    }

    public void Open()
    {
        if (!TryGetShortPath(longpath, out shortName))
            throw new Exception("Get short path faield when initializing.");

        aliasName = $"nmci{DateTime.Now.Ticks}";
        MciSendStringWithCheck($"open {shortName} alias {aliasName}", null, 0, IntPtr.Zero);
    }

    public void Close()
    {
        MciSendStringWithCheck($"close {aliasName}", null, 0, IntPtr.Zero);

        aliasName = null;
    }

    public void Play()
    {
        MciSendStringWithCheck($"play {aliasName}", null, 0, IntPtr.Zero);
    }

    public void Resume()
    {
        MciSendStringWithCheck($"resume {aliasName}", null, 0, IntPtr.Zero);
    }

    public void Pause()
    {
        MciSendStringWithCheck($"pause {aliasName}", null, 0, IntPtr.Zero);
    }

    public void Stop()
    {
        MciSendStringWithCheck($"stop {aliasName}", null, 0, IntPtr.Zero);
    }

    public int GetPosition()
    {
        return int.Parse(StatusInfo("position"));
    }

    public int GetLength()
    {
        return int.Parse(StatusInfo("length"));
    }

    public PlaybackState GetState()
    {
        switch (StatusInfo("mode").ToLower())
        {
            case "playing":
                return PlaybackState.Playing;
            case "paused":
                return PlaybackState.Paused;
            case "stopped":
                return PlaybackState.Stopped;
            default:
                return PlaybackState.Invalid;
        }
    }

    public void PlayWait()
    {
        MciSendStringWithCheck($"play {aliasName} wait", null, 0, IntPtr.Zero);
    }

    public void PlayRepeat()
    {
        MciSendStringWithCheck($"play {aliasName} repeat", null, 0, IntPtr.Zero);
    }

    public void Seek(int position)
    {
        MciSendStringWithCheck($"seek {aliasName} to {position}", null, 0, IntPtr.Zero);
    }

    public void SeekToStart()
    {
        MciSendStringWithCheck($"seek {aliasName} to start", null, 0, IntPtr.Zero);
    }

    public void SetSeekMode(bool fExact)
    {
        MciSendStringWithCheck($"set {aliasName} seek exactly {(fExact ? "on" : "off")}", null, 0, IntPtr.Zero);
    }
}
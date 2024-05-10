using System;
using System.IO;

namespace Ursa.Music._163.Utils;

public static class CloseHelper
{
    public static void CloseApplication()
    {
        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");
        string[] files = Directory.GetFiles(dir);

        foreach (var file in files)
        {
            if (file.EndsWith(".m4a"))
            {
                File.Delete(file);
            }
        }

        Environment.Exit(0);
    }
}
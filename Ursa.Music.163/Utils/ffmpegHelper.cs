using System;
using System.IO;
using System.Threading;

namespace Ursa.Music._163.Utils;

public class ffmpegHelper
{
    private static string ffmpegPath { get; set; } =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "ffmpeg.exe");

    /// <summary>
    /// 音频运行格式转换( .m4a => .mp3 )
    /// </summary>
    /// <param name="ffmpegVirtualPath">ffmpeg.exe本地文件的存放路径</param>
    /// <param name="sourceFile">m4a源文件物理路径</param>
    /// <param name="fileVirtualPath">mp3目标文件虚拟路径</param>
    /// <returns></returns>
    public static string ConvertAudio(string sourceFile, string fileVirtualPath, string? ffmpegVirtualPath = null)
    {
        ffmpegVirtualPath ??= ffmpegPath;
        //取得ffmpeg.exe的物理路径
        string ffmpeg = ffmpegVirtualPath;
        if (!File.Exists(ffmpeg))
        {
            return "找不到格式转换程序！";
        }

        if (!File.Exists(sourceFile))
        {
            return "找不到源文件！";
        }

        string destFile = fileVirtualPath;
        System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
        FilestartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

        /*ffmpeg参数说明
         * -i 1.avi   输入文件
         * -ab/-ac <比特率> 设定声音比特率，前面-ac设为立体声时要以一半比特率来设置，比如192kbps的就设成96，转换
            均默认比特率都较小，要听到较高品质声音的话建议设到160kbps（80）以上
         * -ar <采样率> 设定声音采样率，PSP只认24000
         * -b <比特率> 指定压缩比特率，似乎ffmpeg是自动VBR的，指定了就大概是平均比特率，比如768，1500这样的   --加了以后转换不正常
         * -r 29.97 桢速率（可以改，确认非标准桢率会导致音画不同步，所以只能设定为15或者29.97）
         * s 320x240 指定分辨率
         * 最后的路径为目标文件
         */

        //FilestartInfo.Arguments = " -i " + sourceFile + " -vn -ar 8 -ac 2 -ab 192 -f mp3 " + destFile;      
        FilestartInfo.Arguments = " -i " + sourceFile + " -f mp3 " + destFile;

        try
        {
            //转换
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(FilestartInfo);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            // process.StartInfo.CreateNoWindow = true;

            StreamWriter q1 = process.StandardInput;
            StreamReader q2 = process.StandardOutput;
            process.StandardInput.Close();
            process.StandardOutput.Close();
            process.Close();
            q1.Close();
        }
        catch (Exception ex)
        {
            Thread.Sleep(1000);
            if (File.Exists(fileVirtualPath))
            {
                return "格式转换成功！";
            }

            return "格式转换失败！";
        }

        return "格式转换成功！";
    }
}
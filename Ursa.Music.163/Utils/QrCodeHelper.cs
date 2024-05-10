using System;
using System.Drawing;
using System.IO;
using Avalonia.Platform;
using Ursa.Music._163.Converter;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Ursa.Music._163.Utils;

public class QrCodeHelper
{
    /// <summary>
    /// 生成二维码  
    /// </summary>
    /// <param name="msg">信息</param>
    /// <param name="version">版本 1 ~ 40</param>
    /// <param name="pixel">像素点大小</param>
    /// <returns>位图</returns>
    public static object? CreateQRCode(string msg, int version, int pixel)
    {
        QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
        QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.M, true,
            true, QRCoder.QRCodeGenerator.EciMode.Utf8, version);

        Bitmap iconBitmap = new Bitmap(AssetLoader.Open(new Uri("avares://Ursa.Music.163/Assets/网易云.png")));
        var iconByte = BitmapToByte(iconBitmap);
        QRCoder.SvgQRCode.SvgLogo icon = new QRCoder.SvgQRCode.SvgLogo(iconByte, 25);

        QRCoder.SvgQRCode svgQrCode = new QRCoder.SvgQRCode(qRCodeData);
        var svgString = svgQrCode.GetGraphic(new Size(200, 200), false,
            QRCoder.SvgQRCode.SizingMode.WidthHeightAttribute, icon);

        string documentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SVG", "lineto_out.png");
        File.WriteAllText(documentPath, svgString);
        var fs = File.OpenRead(documentPath);
        //  var bytes=System.Text.Encoding.Default.GetBytes(svgString);  
        // // var inputBytes = System.Convert.FromBase64String(svgString);
        // using MemoryStream stream = new MemoryStream(fs);

        return null;
        // var ret = QRCoder.SvgQRCodeHelper.GetQRCode(msg, 10, 
        //     "#333333", "#FFFFFF", 
        //     QRCoder.QRCodeGenerator.ECCLevel.M, true,
        //     true, QRCoder.QRCodeGenerator.EciMode.Utf8, -1, true, QRCoder.SvgQRCode.SizingMode.WidthHeightAttribute, icon);
        // using MemoryStream stream = new MemoryStream(bitmap);
        // return new Bitmap(stream);
    }


    public static object? GetQrcode()
    {
        return PathIcon.LoadFromResource(new Uri("avares://Ursa.Music.163/Assets/lineto_out.png"));
    }

    private static byte[] BitmapToByte(Bitmap bitmap)
    {
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        bitmap.Save(ms);
        ms.Seek(0, System.IO.SeekOrigin.Begin);
        byte[] bytes = new byte[ms.Length];
        ms.Read(bytes, 0, bytes.Length);
        ms.Dispose();
        return bytes;
    }
}
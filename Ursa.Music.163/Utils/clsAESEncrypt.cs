using System;
using System.Security.Cryptography;
using System.Text;

namespace Ursa.Music._163.Utils;

class clsAESEncrypt
{
    public clsAESEncrypt()
    {
    }

    /// <summary>
    /// AES加密 
    /// </summary>
    /// <param name="text">明文</param>
    /// <param name="password">密钥</param>
    /// <param name="iv">向量</param>
    /// <returns></returns>
    public string AESEncrypt(string text, string password, string iv)
    {
        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.Mode = CipherMode.CBC;
        rijndaelManaged.Padding = PaddingMode.PKCS7;
        rijndaelManaged.KeySize = 128;
        rijndaelManaged.BlockSize = 128;

        byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);
        byte[] keyBytes = new byte[16];
        int len = pwdBytes.Length;
        if (len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        System.Array.Copy(pwdBytes, keyBytes, len);
        rijndaelManaged.Key = keyBytes;

        byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
        rijndaelManaged.IV = ivBytes;

        ICryptoTransform iCryptoTransform = rijndaelManaged.CreateEncryptor();
        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        byte[] targetBytes = iCryptoTransform.TransformFinalBlock(textBytes, 0, textBytes.Length);
        return Convert.ToBase64String(targetBytes);
    }

    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="text">密文</param>
    /// <param name="password">密钥</param>
    /// <param name="iv">向量</param>
    /// <returns></returns>
    public string AESDecrypt(string text, string password, string iv)
    {
        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.Mode = CipherMode.CBC;
        rijndaelManaged.Padding = PaddingMode.PKCS7;
        rijndaelManaged.KeySize = 128;
        rijndaelManaged.BlockSize = 128;

        byte[] encryptedData = Convert.FromBase64String(text);
        byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);
        byte[] keyBytes = new byte[16];
        int len = pwdBytes.Length;
        if (len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        System.Array.Copy(pwdBytes, keyBytes, len);
        rijndaelManaged.Key = keyBytes;

        byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
        rijndaelManaged.IV = ivBytes;

        ICryptoTransform iCryptoTransform = rijndaelManaged.CreateDecryptor();
        byte[] targetBytes = iCryptoTransform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        return Encoding.UTF8.GetString(targetBytes);
    }
}
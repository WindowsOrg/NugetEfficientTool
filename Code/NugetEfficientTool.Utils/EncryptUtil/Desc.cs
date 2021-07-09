using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NugetEfficientTool.Utils
{
    public static class Desc
    {
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
            byte[] keyBytes = Encoding.Unicode.GetBytes(key);//定义字节数组用来存储密钥
            if (keyBytes.Length != 8)
            {
                throw new ArgumentException("密钥字节长度不等于8，无效");
            }
            byte[] data = Encoding.Unicode.GetBytes(str);//定义字节数组，用来存储加密对象
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, descsp.CreateEncryptor(keyBytes, keyBytes), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();//释放解秘流
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string str, string key)
        {
            //实例化加密对象
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
            byte[] keyBytes = Encoding.Unicode.GetBytes(key);//定义字节数组用来存储密钥
            byte[] strBytes = Convert.FromBase64String(str);//定义字节数组，用来存储加密对象
            if (keyBytes.Length != 8)
            {
                throw new ArgumentException("密钥字节长度不等于8，无效");
            }
            //实例化内存对象
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream stream2 = new CryptoStream(stream, descsp.CreateDecryptor(keyBytes, keyBytes), CryptoStreamMode.Write))
                {
                    stream2.Write(strBytes, 0, strBytes.Length);
                    stream2.FlushFinalBlock();//释放解秘流
                }
                return Encoding.Unicode.GetString(stream.ToArray());
            }
        }
    }
}

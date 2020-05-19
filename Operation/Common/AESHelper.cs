using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Operation.Common
{
    public class AESHelper
    {
        /// <summary>
        /// 默认密钥-密钥的长度必须是32
        /// </summary>
        private const string PublicKey = "PFVhaGmxiTCxKUCjfVB30qQrr6nWrpGq";

        /// <summary>
        /// 默认向量
        /// </summary>
        private const string IV = "Irjl4yMJqePXb82H";

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string str)
        {
            Byte[] plainBytes = Encoding.UTF8.GetBytes(str);
            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(PublicKey.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(IV.PadRight(bVector.Length)), bVector, bVector.Length);
            Byte[] Cryptograph = null; // 加密后的密文
            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流
                using (MemoryStream Memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Encryptor = new CryptoStream(Memory,
                    Aes.CreateEncryptor(bKey, bVector),
                    CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流
                        Encryptor.Write(plainBytes, 0, plainBytes.Length);
                        Encryptor.FlushFinalBlock();
                        Cryptograph = Memory.ToArray();
                    }
                }
            }
            catch
            {
                Cryptograph = null;
            }
            return Convert.ToBase64String(Cryptograph);
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt(string str)
        {
            Byte[] encryptedBytes = Convert.FromBase64String(str);
            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(PublicKey.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(IV.PadRight(bVector.Length)), bVector, bVector.Length);
            Byte[] original = null; // 解密后的明文
            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流，存储密文
                using (MemoryStream Memory = new MemoryStream(encryptedBytes))
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Decryptor = new CryptoStream(Memory,
                    Aes.CreateDecryptor(bKey, bVector),
                    CryptoStreamMode.Read))
                    {
                        // 明文存储区
                        using (MemoryStream originalMemory = new MemoryStream())
                        {
                            Byte[] Buffer = new Byte[1024];
                            Int32 readBytes = 0;
                            while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                originalMemory.Write(Buffer, 0, readBytes);
                            }
                            original = originalMemory.ToArray();
                        }
                    }
                }
            }
            catch
            {
                original = null;
            }
            return Encoding.UTF8.GetString(original);
        }
    }
}

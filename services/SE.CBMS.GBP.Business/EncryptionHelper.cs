using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SE.CBMS.GBP.Business
{
    public class EncryptionHelper
    {
        
        public static string Encrypt(string value, string key)
        {
            if (string.IsNullOrEmpty(value) == false)
            {


                #pragma warning disable SYSLIB0021 // Type or member is obsolete
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                   #pragma warning restore SYSLIB0021 // Type or member is obsolete


                des.IV = new byte[8];

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(key, new byte[-1 + 1]);

                #pragma warning disable CA1416 // Validate platform compatibility
                des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);
                #pragma warning restore CA1416 // Validate platform compatibility

                MemoryStream ms = new MemoryStream((value.Length * 2) - 1);

                CryptoStream encStream = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

                byte[] plainBytes = Encoding.UTF8.GetBytes(value);

                encStream.Write(plainBytes, 0, plainBytes.Length);

                encStream.FlushFinalBlock();

                byte[] encryptedBytes = new byte[Convert.ToInt32(ms.Length - 1) + 1];

                ms.Position = 0;

                ms.Read(encryptedBytes, 0, Convert.ToInt32(ms.Length));

                encStream.Close();

                value = Convert.ToBase64String(encryptedBytes);

                return value;
            }
            return "";
        }
        public static string Decrypt(string value, string key)
        {

            value = value.Replace(" ", "+");

            #pragma warning disable SYSLIB0021 // Type or member is obsolete
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
              #pragma warning restore SYSLIB0021 // Type or member is obsolete

            des.IV = new byte[8];

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(key, new byte[-1 + 1]);

            #pragma warning disable CA1416 // Validate platform compatibility
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);
            #pragma warning restore CA1416 // Validate platform compatibility

            byte[] encryptedBytes = Convert.FromBase64String(value);

            System.IO.MemoryStream ms = new MemoryStream(value.Length);

            CryptoStream decStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            decStream.Write(encryptedBytes, 0, encryptedBytes.Length);

            decStream.FlushFinalBlock();

            byte[] plainBytes = new byte[Convert.ToInt32(ms.Length - 1) + 1];

            ms.Position = 0;

            ms.Read(plainBytes, 0, Convert.ToInt32(ms.Length));

            decStream.Close();

            return Encoding.UTF8.GetString(plainBytes);

        }
    }
}

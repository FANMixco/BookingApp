using System;
using System.Security.Cryptography;
using System.Text;

namespace BookingApp.DB.Classes
{
    public static class Encryption
    {
        const string HASH = @"9Y]VnfLp<u=KQ6j$";

        public static string Encrypt(string password)
        {
            byte[] data = Encoding.UTF8.GetBytes(password);
            using MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] keys = md5.ComputeHash(Encoding.UTF8.GetBytes(HASH));
            using TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            ICryptoTransform transform = tripleDes.CreateEncryptor();
            byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(results);
        }
    }
}

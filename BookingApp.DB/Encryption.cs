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
            byte[] keys = SHA256.HashData(Encoding.UTF8.GetBytes(HASH));

            using Aes aes = Aes.Create();
            aes.Key = keys;
            aes.Mode = CipherMode.ECB;  // Note: ECB mode is not secure; consider using a more secure mode.
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform transform = aes.CreateEncryptor();
            byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(results);
        }
    }
}

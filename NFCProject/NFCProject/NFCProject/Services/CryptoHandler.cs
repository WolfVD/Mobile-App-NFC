using System.IO;
using System.Security.Cryptography;

namespace NFCProject.Services
{
    public class CryptoHandler
    {
        public byte[] Encrypt(byte[] data, byte[] Key, byte[] IV)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;

                aes.Key = Key;
                aes.IV = IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV)) {
                    return PerformCryptography(data, encryptor);
                }

            }

        }

        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }
    }
}

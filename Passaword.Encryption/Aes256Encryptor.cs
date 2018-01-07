using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Passaword.Encryption.Utils;

namespace Passaword.Encryption
{
    public class Aes256Encryptor : ISymmetricEncryptor
    {
        private readonly ILogger<Aes256Encryptor> _logger;

        public static string Name = "Aes256";

        public Aes256Encryptor(ILogger<Aes256Encryptor> logger)
        {
            _logger = logger;
        }

        public string Encrypt(string plainText, string key)
        {
            var keyBytes = key.FromHex();
            _logger.LogDebug($"Attempting to encrypt {plainText} with key {key}");
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = keyBytes;
                aes.GenerateIV();
                
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt
                            , encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(
                                csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                            return Convert.ToBase64String(aes.IV.Concat(msEncrypt.ToArray()).ToArray());
                        }
                    }
                }
            }
        }

        public string Decrypt(string encryptedSecret, string key)
        {
            return Decrypt(encryptedSecret, new List<string> { key });
        }

        public string Decrypt(string encryptedSecret, IList<string> keys)
        {
            var fullCipher = Convert.FromBase64String(encryptedSecret);

            foreach (var key in keys)
            {
                try
                {
                    _logger.LogDebug($"Attempting to decrypt {encryptedSecret} with key {key}");
                    var iv = new byte[16];
                    var cipher = new byte[fullCipher.Length - iv.Length];

                    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                    Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
                    var keyBytes = key.FromHex();

                    using (var aes = Aes.Create())
                    {
                        aes.KeySize = 256;
                        aes.BlockSize = 128;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.Key = keyBytes;
                        aes.IV = iv;

                        using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                        {
                            using (MemoryStream msDecrypt = new MemoryStream(cipher))
                            {
                                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt
                                    , decryptor, CryptoStreamMode.Read))
                                {
                                    using (StreamReader srDecrypt = new StreamReader(
                                        csDecrypt))
                                    {
                                        return srDecrypt.ReadToEnd();
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //continue
                }
            }
            throw new CryptographicException("Unable to decrypt");
        }
    }
}

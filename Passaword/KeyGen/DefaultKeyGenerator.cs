using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace Passaword.KeyGen
{
    public class DefaultKeyGenerator : IKeyGenerator
    {
        private readonly IConfiguration _config;
        public const string AllowableCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public DefaultKeyGenerator(IConfiguration config)
        {
            _config = config;
        }

        private byte[] GetRandomBytes(int length)
        {
            var bytes = new byte[length];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(bytes);
            }

            return bytes;
        }

        public string GetDefaultEncryptionKey()
        {
            return _config["Passaword:DefaultKey"];
        }

        public IList<string> GetDecryptionKeys()
        {
            var keys = new List<string>();
            var decryptKeys = _config.GetSection("Passaword:DecryptionKeys");

            keys.Add(_config["Passaword:DefaultKey"]);
            if (decryptKeys.Exists())
            {
                keys.AddRange(decryptKeys.Get<string[]>());
            }

            return keys;
        }

        public string GenerateKey(int length = 8, string allowableChars = AllowableCharacters)
        {
            var bytes = GetRandomBytes(length);

            return new string(bytes.Select(x => allowableChars[x % allowableChars.Length]).ToArray());
        }

        public string GenerateSalt(int length = 16)
        {
            var bytes = GetRandomBytes(length);

            return Convert.ToBase64String(bytes);
        }
    }
}

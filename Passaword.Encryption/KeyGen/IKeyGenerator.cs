using System.Collections.Generic;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Passaword.Encryption.KeyGen
{
    public interface IKeyGenerator
    {
        string GenerateKey(int length = 8, string allowableChars = DefaultKeyGenerator.AllowableCharacters);
        string GenerateSalt(int length = 16);
        string GetDefaultEncryptionKey();
        IList<string> GetDecryptionKeys();
        string DeriveKey(string passphrase, byte[] salt, int iterationCount = 10000, KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256, int numBytes = 32);
    }
}

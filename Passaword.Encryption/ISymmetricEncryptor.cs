using System.Collections.Generic;

namespace Passaword.Encryption
{
    public interface ISymmetricEncryptor
    {
        string Encrypt(string plainText, string key);
        string Decrypt(string encryptedSecret, string key);
        string Decrypt(string encryptedSecret, IList<string> keys);
    }
}

using System.Collections.Generic;

namespace Passaword.Encryption
{
    public interface ISecretEncryptor
    {
        string Encrypt(string secret, string key);
        string Decrypt(string encryptedSecret, IList<string> keys);
    }
}

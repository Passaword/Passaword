namespace Passaword.Encryption
{
    public interface ISecretEncryptor
    {
        string Encrypt(string secret, string key);
        string Decrypt(string encryptedSecret, string key);
    }
}

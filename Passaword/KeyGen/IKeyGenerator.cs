namespace Passaword.KeyGen
{
    public interface IKeyGenerator
    {
        string GenerateKey(int length = 8, string allowableChars = DefaultKeyGenerator.AllowableCharacters);
        string GenerateSalt(int length = 16);
        string GetDefaultEncryptionKey();
    }
}

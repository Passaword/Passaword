namespace Passaword
{
    public interface ISecretContextService
    {
        SecretEncryptionContext CreateEncryptionContext();
        SecretDecryptionContext CreateDecryptionContext();
    }
}

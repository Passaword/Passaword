using System.Security.Claims;

namespace Passaword
{
    public interface ISecretContextService
    {
        SecretEncryptionContext CreateEncryptionContext();
        SecretDecryptionContext CreateDecryptionContext();

        SecretEncryptionContext CreateEncryptionContext(ClaimsPrincipal principal);
        SecretDecryptionContext CreateDecryptionContext(ClaimsPrincipal principal);
    }
}

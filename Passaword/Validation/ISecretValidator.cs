using System.Security.Claims;
using Passaword.Constants;

namespace Passaword.Validation
{
    public interface ISecretValidator
    {
        bool Validate(SecretDecryptionContext secretContext, ClaimsPrincipal principal, ValidationStage stage);
        bool ValidateRule(SecretValidationRule rule, ValidationStage stage);
    }
}

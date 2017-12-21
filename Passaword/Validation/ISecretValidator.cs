using System.Security.Claims;
using Passaword.Constants;

namespace Passaword.Validation
{
    public interface ISecretValidator
    {
        ValidationResult Validate(SecretDecryptionContext secretContext, ClaimsPrincipal principal, ValidationStage stage);
        ValidationResult ValidateRule(SecretValidationRule rule, ValidationStage stage);
    }
}

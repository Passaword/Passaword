using Microsoft.AspNetCore.Http;
using Passaword.Constants;

namespace Passaword.Validation
{
    public interface ISecretValidator
    {
        bool Validate(SecretDecryptionContext secretContext, HttpContext httpContext, ValidationStage stage);
        bool ValidateRule(SecretValidationRule rule, ValidationStage stage);
    }
}

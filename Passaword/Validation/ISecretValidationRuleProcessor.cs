using Microsoft.AspNetCore.Http;
using Passaword.Constants;

namespace Passaword.Validation
{
    public interface ISecretValidationRuleProcessor
    {
        string Name { get; }
        ValidationStage ValidationStage { get; set; }
        string SerializeData<T>(T data);
        T DeserializeData<T>(string validationData);
        void CreateRule(SecretEncryptionContext encryptionContext, HttpContext context);
        bool Validate(SecretDecryptionContext secretContext, string validationData, HttpContext context);
    }
}

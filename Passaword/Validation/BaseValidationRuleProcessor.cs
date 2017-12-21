using System;
using System.Security.Claims;
using Newtonsoft.Json;
using Passaword.Constants;

namespace Passaword.Validation
{
    public abstract class BaseValidationRuleProcessor : ISecretValidationRuleProcessor
    {
        public virtual string Name => "Base";
        public virtual ValidationStage ValidationStage { get; set; }
        public virtual string SerializeData<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public virtual T DeserializeData<T>(string validationData)
        {
            return JsonConvert.DeserializeObject<T>(validationData);
        }
        
        public virtual void CreateRule(SecretEncryptionContext encryptionContext, ClaimsPrincipal principal)
        {
            throw new NotImplementedException("Rule setup not implemented");
        }

        public virtual ValidationResult Validate(SecretDecryptionContext secret, string validationData, ClaimsPrincipal principal)
        {
            throw new NotImplementedException("Rule validation not implemented");
        }
    }
}

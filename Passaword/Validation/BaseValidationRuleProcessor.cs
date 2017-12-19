using System;
using Microsoft.AspNetCore.Http;
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
        
        public virtual void CreateRule(SecretEncryptionContext encryptionContext, HttpContext context)
        {
            throw new NotImplementedException("Rule setup not implemented");
        }

        public virtual bool Validate(SecretDecryptionContext secret, string validationData, HttpContext context)
        {
            throw new NotImplementedException("Rule validation not implemented");
        }
    }
}

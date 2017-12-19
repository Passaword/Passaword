using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Passaword.Validation;

namespace Passaword
{
    public class Secret
    {
        public string Id { get; set; }
        public string EncryptedText { get; set; }
        public string EncryptionType { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public string CreatedByProvider { get; set; }

        public ICollection<SecretValidationRule> SecretValidationRules { get; set; } = new List<SecretValidationRule>();
        public ICollection<SecretProperty> SecretProperties { get; set; } = new List<SecretProperty>();
    }

    public class SecretProperty
    {
        public SecretProperty(string type)
        {
            Type = type;
        }

        public string SecretId { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }

        public void SerializeData<T>(T data)
        {
            Data = JsonConvert.SerializeObject(data);
        }

        public T DeserializeData<T>()
        {
            return JsonConvert.DeserializeObject<T>(Data);
        }
    }
}

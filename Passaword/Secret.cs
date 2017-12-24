using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Passaword.Validation;

namespace Passaword
{
    public class Secret
    {
        [MaxLength(32)]
        [Required]
        public string Id { get; set; }
        [Required]
        public string EncryptedText { get; set; }
        [MaxLength(250)]
        [Required]
        public string EncryptionType { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [MaxLength(50)]
        public string CreatedBy { get; set; }
        [MaxLength(50)]
        public string CreatedByProvider { get; set; }

        public ICollection<SecretValidationRule> SecretValidationRules { get; set; } = new List<SecretValidationRule>();
        public ICollection<SecretProperty> SecretProperties { get; set; } = new List<SecretProperty>();
    }

    public class SecretProperty
    {
        public SecretProperty() { }

        public SecretProperty(string type)
        {
            Type = type;
        }

        [MaxLength(32)]
        [Required]
        public string SecretId { get; set; }
        public Secret Secret { get; set; }

        [MaxLength(250)]
        [Required]
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

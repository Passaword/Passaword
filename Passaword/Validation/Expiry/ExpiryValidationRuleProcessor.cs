using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Passaword.Constants;

namespace Passaword.Validation.Expiry
{
    public class ExpiryValidationRuleProcessor : BaseValidationRuleProcessor
    {
        private readonly ILogger<ExpiryValidationRuleProcessor> _logger;

        public ExpiryValidationRuleProcessor(ILogger<ExpiryValidationRuleProcessor> logger)
        {
            _logger = logger;
        }

        public override string Name => "Expiry";

        public bool IsRequired { get; set; }

        public override void CreateRule(SecretEncryptionContext encryptionContext, HttpContext context)
        {
            if (IsRequired && (!encryptionContext.InputData.ContainsKey(UserInputConstants.Expiry) ||
                               string.IsNullOrEmpty(encryptionContext.InputData[UserInputConstants.Expiry])))
            {
                throw new ArgumentException("Expiry is required");
            }
            if (!encryptionContext.InputData.ContainsKey(UserInputConstants.Expiry)) return;

            var expiry = !string.IsNullOrEmpty(encryptionContext.InputData[UserInputConstants.Expiry]) ? 
                DateTime.ParseExact(encryptionContext.InputData[UserInputConstants.Expiry], UserInputConstants.ExpiryDateFormat, CultureInfo.InvariantCulture) : 
                (DateTime?)null;

            var expiryData = new ExpiryValidationData
            {
                Expiry = expiry
            };
            
            encryptionContext.AddValidationRule(new SecretValidationRule
            {
                Validator = this.Name,
                ValidationData = SerializeData(expiryData)
            });
        }

        public override bool Validate(SecretDecryptionContext decryptionContext, string validationData, HttpContext context)
        {
            var expiryData = DeserializeData<ExpiryValidationData>(validationData);

            var isValid = !expiryData.Expiry.HasValue ||  DateTime.Now < expiryData.Expiry;
            _logger.LogDebug($"Validating expiry: {isValid}");
            return isValid;
        }
    }
}

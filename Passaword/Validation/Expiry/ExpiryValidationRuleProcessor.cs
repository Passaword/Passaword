using System;
using System.Globalization;
using System.Security.Claims;
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

        public override void CreateRule(SecretEncryptionContext encryptionContext, ClaimsPrincipal principal)
        {
            if (IsRequired && (!encryptionContext.GetInput<DateTime?>(UserInputConstants.Expiry).HasValue))
            {
                throw new ArgumentException("Expiry is required");
            }
            if (!encryptionContext.GetInput<DateTime?>(UserInputConstants.Expiry).HasValue) return;

            var expiry = encryptionContext.GetInput<DateTime>(UserInputConstants.Expiry);

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

        public override ValidationResult Validate(SecretDecryptionContext decryptionContext, string validationData, ClaimsPrincipal principal)
        {
            var expiryData = DeserializeData<ExpiryValidationData>(validationData);

            var isValid = !expiryData.Expiry.HasValue ||  DateTime.Now < expiryData.Expiry;
            _logger.LogDebug($"Validating expiry: {isValid}");
            if (isValid)
            {
                return ValidationResult.SuccessResult;
            }
            else
            {
                return new ValidationResult(false)
                {
                    Error = "Secret expired",
                    ValidationPointOfFailure = this.Name
                };
            }
        }
    }
}

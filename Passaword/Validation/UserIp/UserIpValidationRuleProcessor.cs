using System;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using Passaword.Constants;
using Passaword.KeyGen;
using Passaword.Utils;

namespace Passaword.Validation.UserIp
{
    public class UserIpValidationRuleProcessor : BaseValidationRuleProcessor
    {
        private readonly ILogger<UserIpValidationRuleProcessor> _logger;

        public UserIpValidationRuleProcessor(ILogger<UserIpValidationRuleProcessor> logger)
        {
            _logger = logger;
        }

        public override string Name => "UserIp";
        public bool IsRequired { get; set; }
        
        public override void CreateRule(SecretEncryptionContext encryptionContext, ClaimsPrincipal principal)
        {
            if (IsRequired && (string.IsNullOrEmpty(encryptionContext.GetInput(UserInputConstants.IpRegex))))
            {
                throw new ArgumentException("IP is required");
            }
            if (encryptionContext.GetInput(UserInputConstants.IpRegex) == null) return;

            var ip = encryptionContext.GetInput(UserInputConstants.IpRegex);
            var ipData = new UserIpValidationData
            {
                IpRegex = ip
            };
            
            _logger.LogDebug($"Setting IP regex to {ip}");
            encryptionContext.AddValidationRule(new SecretValidationRule
            {
                Validator = this.Name,
                ValidationData = SerializeData(ipData)
            });
        }

        public override ValidationResult Validate(SecretDecryptionContext decryptionContext, string validationData, ClaimsPrincipal principal)
        {
            var ipData = DeserializeData<UserIpValidationData>(validationData);
            string ipAddress = decryptionContext.GetInput(UserInputConstants.IpAddress);
            if (string.IsNullOrEmpty(ipData.IpRegex) || string.IsNullOrEmpty(ipAddress)) return ValidationResult.SuccessResult;

            if (!Regex.IsMatch(ipAddress, ipData.IpRegex))
            {
                _logger.LogDebug($"IP address {ipAddress} did not match pattern {ipData.IpRegex}");

                return new ValidationResult(false)
                {
                    Error = "User IP does not match expected pattern",
                    ValidationPointOfFailure = this.Name
                };
            }
            
            return ValidationResult.SuccessResult;
        }
    }
}

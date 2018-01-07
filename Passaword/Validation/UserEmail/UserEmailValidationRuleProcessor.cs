using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using Passaword.Constants;
using Passaword.Utils;

namespace Passaword.Validation.UserEmail
{
    public class UserEmailValidationRuleProcessor : BaseValidationRuleProcessor
    {
        private readonly ILogger<UserEmailValidationRuleProcessor> _logger;

        public UserEmailValidationRuleProcessor(ILogger<UserEmailValidationRuleProcessor> logger)
        {
            _logger = logger;
        }

        public override string Name => "UserEmail";
        public bool IsRequired { get; set; }
        
        public override void CreateRule(SecretEncryptionContext encryptionContext, ClaimsPrincipal principal)
        {
            if (IsRequired && (string.IsNullOrEmpty(encryptionContext.GetInput(UserInputConstants.EmailAddress))))
            {
                throw new ArgumentException("Email is required");
            }
            if (encryptionContext.GetInput(UserInputConstants.EmailAddress) == null) return;

            var email = encryptionContext.GetInput(UserInputConstants.EmailAddress);
            var emailData = new UserEmailValidationData
            {
                Email = email,
                MustLogin = encryptionContext.GetInput<bool>(UserInputConstants.ForceAuthentication)
            };
            
            _logger.LogDebug($"Setting user email to {email}");
            encryptionContext.AddValidationRule(new SecretValidationRule
            {
                Validator = this.Name,
                ValidationData = SerializeData(emailData)
            });
        }

        public override ValidationResult Validate(SecretDecryptionContext decryptionContext, string validationData, ClaimsPrincipal principal)
        {
            var emailData = DeserializeData<UserEmailValidationData>(validationData);
            if (emailData.MustLogin)
            {
                if (principal == null || !principal.Identity.IsAuthenticated)
                {
                    _logger.LogDebug($"User is not authorized");

                    return new ValidationResult(false)
                    {
                        Error = "User is not authenticated",
                        ValidationPointOfFailure = this.Name
                    };
                }

                var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;

                if (userEmail == emailData.Email)
                {
                    return ValidationResult.SuccessResult;
                }
                else
                {
                    _logger.LogDebug($"Authenticated user {userEmail} did not match {emailData.Email}");
                    return new ValidationResult(false)
                    {
                        Error = "Authenticated user does not match target user",
                        ValidationPointOfFailure = this.Name
                    };
                }
            }
            return ValidationResult.SuccessResult;
        }
    }
}

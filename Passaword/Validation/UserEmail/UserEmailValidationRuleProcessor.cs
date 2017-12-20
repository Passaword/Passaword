using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using Passaword.Constants;
using Passaword.KeyGen;
using Passaword.Utils;

namespace Passaword.Validation.UserEmail
{
    public class UserEmailValidationRuleProcessor : BaseValidationRuleProcessor
    {
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogger<UserEmailValidationRuleProcessor> _logger;

        public UserEmailValidationRuleProcessor(IKeyGenerator keyGenerator, ILogger<UserEmailValidationRuleProcessor> logger)
        {
            _keyGenerator = keyGenerator;
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

        public override bool Validate(SecretDecryptionContext decryptionContext, string validationData, ClaimsPrincipal principal)
        {
            var emailData = DeserializeData<UserEmailValidationData>(validationData);
            if (emailData.MustLogin)
            {
                if (principal == null || !principal.Identity.IsAuthenticated)
                {
                    return false;
                }

                var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;

                return userEmail == emailData.Email;
            }
            return true;
        }
    }
}

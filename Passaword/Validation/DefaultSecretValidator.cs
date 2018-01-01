using System;
using System.Linq;
using System.Security.Claims;
using Passaword.Constants;

namespace Passaword.Validation
{
    public class DefaultSecretValidator : ISecretValidator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PassawordContext _context;
        private SecretDecryptionContext _secretContext;
        private ClaimsPrincipal _principal;

        public DefaultSecretValidator(IServiceProvider serviceProvider, PassawordContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        public virtual ValidationResult Validate(SecretDecryptionContext secretContext, ClaimsPrincipal principal, ValidationStage stage)
        {
            _secretContext = secretContext;
            _principal = principal;

            foreach (var rule in _secretContext.Secret.SecretValidationRules)
            {
                var result = ValidateRule(rule, stage);
                if (!result.IsValid)
                {
                    return result;
                }
            }
            return ValidationResult.SuccessResult;
        }

        public virtual ValidationResult ValidateRule(SecretValidationRule rule, ValidationStage stage)
        {
            var validator = _context.SecretValidationRuleProcessors.FirstOrDefault(q => q.Name == rule.Validator);
            if (validator == null) throw new Exception($"Validation rule type {rule.Validator} not registered");

            if (stage == ValidationStage.AfterGet && validator.ValidationStage != stage)
                return ValidationResult.SuccessResult;

            return validator.Validate(_secretContext, rule.ValidationData, _principal);
        }
    }
}

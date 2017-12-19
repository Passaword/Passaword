﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Passaword.Constants;

namespace Passaword.Validation
{
    public class DefaultSecretValidator : ISecretValidator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PassawordContext _context;
        private SecretDecryptionContext _secretContext;
        private HttpContext _httpContext;

        public DefaultSecretValidator(IServiceProvider serviceProvider, PassawordContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        public virtual bool Validate(SecretDecryptionContext secretContext, HttpContext httpContext, ValidationStage stage)
        {
            _secretContext = secretContext;
            _httpContext = httpContext;

            foreach (var rule in _secretContext.Secret.SecretValidationRules)
            {
                if (!ValidateRule(rule, stage))
                {
                    return false;
                }
            }
            return true;
        }

        public virtual bool ValidateRule(SecretValidationRule rule, ValidationStage stage)
        {
            var validator = _context.SecretValidationRuleProcessors.FirstOrDefault(q => q.Name == rule.Validator);
            if (validator == null) throw new Exception($"Validation rule type {rule.Validator} not registered");

            if (validator.ValidationStage != stage) return true;

            return validator.Validate(_secretContext, rule.ValidationData, _httpContext);
        }
    }
}
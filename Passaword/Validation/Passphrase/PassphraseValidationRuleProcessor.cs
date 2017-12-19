﻿using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Passaword.Constants;
using Passaword.KeyGen;
using Passaword.Utils;

namespace Passaword.Validation.Passphrase
{
    public class PassphraseValidationRuleProcessor : BaseValidationRuleProcessor
    {
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogger<PassphraseValidationRuleProcessor> _logger;

        public PassphraseValidationRuleProcessor(IKeyGenerator keyGenerator, ILogger<PassphraseValidationRuleProcessor> logger)
        {
            _keyGenerator = keyGenerator;
            _logger = logger;
        }

        public override string Name => "Passphrase";
        public bool IsRequired { get; set; }

        private string GetEncryptionKey(string passphrase, PassphraseValidationData options)
        {
            var key = KeyDerivation.Pbkdf2(
                password: passphrase,
                salt: Convert.FromBase64String(options.Salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: options.IterationCount,
                numBytesRequested: 32
            );
            return key.ToHex();
        }

        public override void CreateRule(SecretEncryptionContext encryptionContext, HttpContext context)
        {
            if (IsRequired && (!encryptionContext.InputData.ContainsKey(UserInputConstants.Passphrase) ||
                               string.IsNullOrEmpty(encryptionContext.InputData[UserInputConstants.Passphrase])))
            {
                throw new ArgumentException("Passphrase is required");
            }
            if (!encryptionContext.InputData.ContainsKey(UserInputConstants.Passphrase)) return;

            var passphrase = encryptionContext.InputData[UserInputConstants.Passphrase];
            var passphraseData = new PassphraseValidationData
            {
                Algorithm = PassphraseAlgorithm.Pbkdf2Sha1,
                IterationCount = 10000,
                Salt = _keyGenerator.GenerateSalt()
            };

            encryptionContext.EncryptionKey = GetEncryptionKey(passphrase, passphraseData);
            _logger.LogDebug($"Using passphrase {passphrase} to set encryption key to {encryptionContext.EncryptionKey}");
            encryptionContext.AddValidationRule(new SecretValidationRule
            {
                Validator = this.Name,
                ValidationData = SerializeData(passphraseData)
            });
        }

        public override bool Validate(SecretDecryptionContext decryptionContext, string validationData, HttpContext context)
        {
            var passphraseData = DeserializeData<PassphraseValidationData>(validationData);

            var userSuppliedPassphrase = decryptionContext.InputData[UserInputConstants.Passphrase];

            switch (passphraseData.Algorithm)
            {
                case PassphraseAlgorithm.Pbkdf2Sha1:
                default:
                    decryptionContext.EncryptionKey = GetEncryptionKey(userSuppliedPassphrase, passphraseData);
                    _logger.LogDebug($"Using passphrase {userSuppliedPassphrase} to set encryption key to {decryptionContext.EncryptionKey}");
                    return true;
            }

        }
    }
}

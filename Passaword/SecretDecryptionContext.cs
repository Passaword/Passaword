using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Passaword.Constants;
using Passaword.Encryption;
using Passaword.Events;
using Passaword.KeyGen;
using Passaword.Storage;
using Passaword.Validation;

namespace Passaword
{
    public class SecretDecryptionContext : IDisposable
    {
        private readonly ISecretStore _secretStore;
        private readonly ISecretEncryptor _secretEncryptor;
        private readonly ISecretValidator _secretValidator;
        private readonly IServiceProvider _serviceProvider;
        private readonly PassawordContext _context;
        private readonly DecryptionEventArgs _decryptEventArgs;
        private readonly DecryptionFailedEventArgs _decryptFailedEventArgs;
        private readonly ILogger<SecretDecryptionContext> _logger;

        public SecretDecryptionContext(
            IKeyGenerator keyGenerator,
            ISecretStore secretStore,
            ISecretEncryptor secretEncryptor,
            ISecretValidator secretValidator,
            IServiceProvider serviceProvider,
            PassawordContext context,
            DecryptionEventArgs decryptEventArgs,
            DecryptionFailedEventArgs decryptFailedEventArgs,
            ILogger<SecretDecryptionContext> logger)
        {
            _secretStore = secretStore;
            _secretEncryptor = secretEncryptor;
            _secretValidator = secretValidator;
            _serviceProvider = serviceProvider;
            _context = context;
            _decryptEventArgs = decryptEventArgs;
            _decryptFailedEventArgs = decryptFailedEventArgs;
            _logger = logger;

            _decryptEventArgs.Context = this;
            _decryptFailedEventArgs.Context = this;
            EncryptionKey = keyGenerator.GetDefaultEncryptionKey();
        }

        public Secret Secret { get; set; }
        public string EncryptionKey { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public IDictionary<string, object> InputData { get; set; } = new Dictionary<string, object>();

        public virtual string GetInput(string key)
        {
            return GetInput<string>(key);
        }

        public virtual T GetInput<T>(string key)
        {
            if (!InputData.ContainsKey(key)) return default(T);

            var value = InputData[key];
            if (value is T variable)
            {
                return variable;
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
        }

        public virtual string DecryptSecret()
        {
            var decryptorType = Type.GetType(Secret.EncryptionType);
            if (decryptorType == null) throw new Exception($"Could not find encryptor type {Secret.EncryptionType}");
            var decryptor = _serviceProvider.GetService(decryptorType) as ISecretEncryptor;
            if (decryptor == null) throw new Exception($"Encryption type {Secret.EncryptionType} does not inherit from ISecretEncryptor");

            return _secretEncryptor.Decrypt(Secret.EncryptedText, EncryptionKey);
        }

        public virtual ValidationResult ValidateSecret(ValidationStage stage)
        {
            return _secretValidator.Validate(this, Principal, stage);
        }

        public virtual async Task<ValidationResult> PreProcessAsync(string id)
        {
            var secret = await _secretStore.GetAsync(id);
            Secret = secret ?? throw new KeyNotFoundException("Invalid secret ID");

            ValidationResult result = ValidateSecret(ValidationStage.AfterGet);
            _decryptEventArgs.ValidationResult = result;
            if (!result.IsValid)
            {
                _context.OnPreValidationFailed?.Invoke(this, _decryptFailedEventArgs);
            }
            return result;
        }

        public virtual async Task IncrementFailedDecryptions()
        {
            var failedDecryptions = Secret.SecretProperties.FirstOrDefault(q => q.Type == SecretProperties.FailedDecryptions);
            if (failedDecryptions == null)
            {
                failedDecryptions = new SecretProperty(SecretProperties.FailedDecryptions)
                {
                    SecretId = Secret.Id
                };
                failedDecryptions.SerializeData(1);
                Secret.SecretProperties.Add(failedDecryptions);
            }
            failedDecryptions.SerializeData(failedDecryptions.DeserializeData<int>() + 1);
            await _secretStore.UpdateAsync(Secret);
        }

        public virtual async Task<string> DecryptSecretAsync(string id)
        {
            var secret = await _secretStore.GetAsync(id);
            if (secret == null)
            {
                _decryptFailedEventArgs.FailureReason = "Invalid secret ID";
                _context.OnDecryptionFailed?.Invoke(this, _decryptFailedEventArgs);
                return null;
            }

            Secret = secret;
            var result = ValidateSecret(ValidationStage.BeforeDecrypt);
            if (result.IsValid)
            {
                try
                {
                    var decrypted = DecryptSecret();
                    await _secretStore.DeleteAsync(Secret.Id);
                    _context.OnSecretDecrypted?.Invoke(this, _decryptEventArgs);
                    return decrypted;
                }
                catch (Exception e)
                {
                    await IncrementFailedDecryptions();

                    _decryptFailedEventArgs.FailureReason = e.Message;
                    _decryptFailedEventArgs.Exception = e;
                    _decryptFailedEventArgs.ValidationResult = new ValidationResult(false)
                    {
                        ValidationPointOfFailure = "Decryption",
                        Error = "Decryption failed"
                    };
                    _context.OnDecryptionFailed?.Invoke(this, _decryptFailedEventArgs);
                    return null;
                }
            }
            else
            {
                await IncrementFailedDecryptions();
                _decryptFailedEventArgs.ValidationResult = result;
                _decryptFailedEventArgs.FailureReason = "Validation failed";
                _context.OnDecryptionFailed?.Invoke(this, _decryptFailedEventArgs);
                return null;
            }
        }
        
        public void Dispose()
        {
            InputData = null;
            EncryptionKey = null;
            Secret = null;
        }
    }
}

using System;
using System.Collections.Generic;
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
    public class SecretEncryptionContext : IDisposable
    {
        private readonly ISecretEncryptor _secretEncryptor;
        private readonly ISecretStore _secretStore;
        private readonly PassawordContext _context;
        private readonly EncryptionEventArgs _eventArgs;
        private readonly ILogger<SecretEncryptionContext> _logger;

        public SecretEncryptionContext(
            IKeyGenerator keyGenerator, 
            ISecretEncryptor secretEncryptor,
            ISecretStore secretStore,
            PassawordContext context,
            EncryptionEventArgs eventArgs,
            ILogger<SecretEncryptionContext> logger)
        {
            _secretEncryptor = secretEncryptor;
            _secretStore = secretStore;
            _context = context;
            _eventArgs = eventArgs;
            _logger = logger;

            _eventArgs.Context = this;
            EncryptionKey = keyGenerator.GetDefaultEncryptionKey();
        }

        public string EncryptionKey { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public IDictionary<string, object> InputData { get; set; } = new Dictionary<string, object>();
        public IList<SecretProperty> SecretProperties { get; set; } = new List<SecretProperty>();
        public string UnencryptedSecret => InputData.ContainsKey(UserInputConstants.Secret) ? GetInput(UserInputConstants.Secret) : "";
        public Secret Secret { get; set; } = new Secret();

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

        public virtual void AddRules()
        {
            foreach (var rule in _context.SecretValidationRuleProcessors)
            {
                _logger.LogInformation($"Adding validation rule {rule.Name} to context");
                rule.CreateRule(this, Principal);
            }
        }

        public void AddValidationRule(SecretValidationRule rule)
        {
            Secret.SecretValidationRules.Add(rule);
        }

        public virtual void AddProperties()
        {
            foreach(var property in SecretProperties)
                Secret.SecretProperties.Add(property);
        }

        public virtual void EncryptSecret()
        {
            Secret.EncryptedText = _secretEncryptor.Encrypt(UnencryptedSecret, EncryptionKey);
            Secret.EncryptionType = _secretEncryptor.GetType().AssemblyQualifiedName;
        }

        public virtual async Task<String> EncryptSecretAsync()
        {
            AddRules();
            AddProperties();
            EncryptSecret();
            string id  = await _secretStore.CreateAsync(Secret);
            _context.OnSecretEncrypted?.Invoke(this, _eventArgs);
            return id;
        }

        public void Dispose()
        {
            InputData?.Clear();
            InputData = null;
            EncryptionKey = null;
            Secret = null;
            SecretProperties?.Clear();
            SecretProperties = null;
        }
    }
}

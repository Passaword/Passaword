using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISecretEncryptor _secretEncryptor;
        private readonly ISecretStore _secretStore;
        private readonly PassawordContext _context;
        private readonly EncryptionEventArgs _eventArgs;
        private readonly ILogger<SecretEncryptionContext> _logger;

        public SecretEncryptionContext(
            IKeyGenerator keyGenerator, 
            IHttpContextAccessor httpContextAccessor,
            ISecretEncryptor secretEncryptor,
            ISecretStore secretStore,
            PassawordContext context,
            EncryptionEventArgs eventArgs,
            ILogger<SecretEncryptionContext> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _secretEncryptor = secretEncryptor;
            _secretStore = secretStore;
            _context = context;
            _eventArgs = eventArgs;
            _logger = logger;

            _eventArgs.Context = this;
            EncryptionKey = keyGenerator.GetDefaultEncryptionKey();
        }

        public string EncryptionKey { get; set; }
        public HttpContext HttpContext => _httpContextAccessor.HttpContext;
        public IDictionary<string, string> InputData { get; set; } = new Dictionary<string, string>();
        public IList<SecretProperty> SecretProperties { get; set; } = new List<SecretProperty>();
        public string UnencryptedSecret => InputData.ContainsKey(UserInputConstants.Secret) ? InputData[UserInputConstants.Secret] : "";
        public Secret Secret { get; set; } = new Secret();

        public virtual void AddRules()
        {
            foreach (var rule in _context.SecretValidationRuleProcessors)
            {
                _logger.LogInformation($"Adding validation rule {rule.Name} to context");
                rule.CreateRule(this, _httpContextAccessor.HttpContext);
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

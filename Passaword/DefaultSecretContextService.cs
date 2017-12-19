using System;

namespace Passaword
{
    public class DefaultSecretContextService : ISecretContextService
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultSecretContextService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public SecretEncryptionContext CreateEncryptionContext()
        {
            return (SecretEncryptionContext) _serviceProvider.GetService(typeof(SecretEncryptionContext));
        }

        public SecretDecryptionContext CreateDecryptionContext()
        {
            return (SecretDecryptionContext)_serviceProvider.GetService(typeof(SecretDecryptionContext));
        }
    }
}

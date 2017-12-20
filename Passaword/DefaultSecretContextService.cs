using System;
using System.Security.Claims;

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

        public SecretEncryptionContext CreateEncryptionContext(ClaimsPrincipal principal)
        {
            var ctx = CreateEncryptionContext();
            ctx.Principal = principal;
            return ctx;
        }

        public SecretDecryptionContext CreateDecryptionContext(ClaimsPrincipal principal)
        {
            var ctx = CreateDecryptionContext();
            ctx.Principal = principal;
            return ctx;
        }
    }
}

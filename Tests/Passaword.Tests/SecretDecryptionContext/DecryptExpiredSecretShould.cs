using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Passaword.Configuration;
using Passaword.Constants;
using Passaword.Encryption;
using Xunit;

namespace Passaword.Tests.SecretDecryptionContext
{
    public class DecryptExpiredSecretShould
    {
        private readonly TestContext _ctx;

        public DecryptExpiredSecretShould()
        {
            _ctx = new TestContext();
            _ctx.Configure();
            _ctx.Services.AddPassaword()
                .AddExpiryValidation();
            _ctx.Build();
        }
        
        [Fact]
        public async void NotBeDecryptable()
        {
            string secretId;

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                context.InputData.Add(UserInputConstants.Expiry, DateTime.Now.AddDays(-1));
                secretId = await context.EncryptSecretAsync();
            }

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretDecryptionContext>())
            {
                Assert.Null(await context.DecryptSecretAsync(secretId));
            }
        }
        
    }
}

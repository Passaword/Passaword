using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Passaword.Configuration;
using Passaword.Constants;
using Passaword.Encryption;
using Xunit;

namespace Passaword.Tests.SecretDecryptionContext
{
    public class DecryptSecretShould
    {
        private readonly TestContext _ctx;

        public DecryptSecretShould()
        {
            _ctx = new TestContext();
            _ctx.ConfigureAndBuild();
        }

        [Fact]
        public async void ReturnCorrectSecret()
        {
            string secretId;

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                secretId = await context.EncryptSecretAsync();
            }

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretDecryptionContext>())
            {
                Assert.Equal("test", await context.DecryptSecretAsync(secretId));
            }
        }

        [Fact]
        public async void OnlyReturnOnce()
        {
            string secretId;

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                secretId = await context.EncryptSecretAsync();
            }

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretDecryptionContext>())
            {
                Assert.Equal("test", await context.DecryptSecretAsync(secretId));

                Assert.Null(await context.DecryptSecretAsync(secretId));
            }
        }
        
    }
}

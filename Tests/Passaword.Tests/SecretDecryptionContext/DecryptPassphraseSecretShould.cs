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
    public class DecryptPassphraseSecretShould
    {
        private readonly TestContext _ctx;

        public DecryptPassphraseSecretShould()
        {
            _ctx = new TestContext();
            _ctx.Configure();
            _ctx.Services.AddPassaword()
                .AddPassphraseValidation();
            _ctx.Build();
        }
        
        [Fact]
        public async void BeDecryptable()
        {
            string secretId;

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                context.InputData.Add(UserInputConstants.Passphrase, "test");
                secretId = await context.EncryptSecretAsync();
            }

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretDecryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Passphrase, "test");
                Assert.Equal("test", await context.DecryptSecretAsync(secretId));
            }
        }

        [Fact]
        public async void RequireCorrectPassphrase()
        {
            string secretId;

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                context.InputData.Add(UserInputConstants.Passphrase, "test");
                secretId = await context.EncryptSecretAsync();
            }

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretDecryptionContext>())
            {
                Assert.Null(await context.DecryptSecretAsync(secretId));

                context.InputData.Add(UserInputConstants.Passphrase, "some other passphrase");

                Assert.Null(await context.DecryptSecretAsync(secretId));

                context.InputData[UserInputConstants.Passphrase] = "test";
                Assert.Equal("test", await context.DecryptSecretAsync(secretId));

            }
        }
    }
}

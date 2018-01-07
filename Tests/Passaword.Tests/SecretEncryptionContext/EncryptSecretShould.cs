using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Passaword.Configuration;
using Passaword.Constants;
using Passaword.Encryption;
using Xunit;

namespace Passaword.Tests.SecretEncryptionContext
{
    public class EncryptSecretShould
    {
        private readonly TestContext _ctx;

        public EncryptSecretShould()
        {
            _ctx = new TestContext();
            _ctx.ConfigureAndBuild();
        }

        [Fact]
        public async void ReturnRandomString()
        {
            string secretId;

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");

                secretId = await context.EncryptSecretAsync();
                Assert.True(secretId != null, "Secret is null");
                Assert.NotEqual(secretId, await context.EncryptSecretAsync());
            }
        }

        [Fact]
        public async void ScrambleSecret()
        {
            string secretId;

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");

                secretId = await context.EncryptSecretAsync();

                Assert.NotEqual("test", context.Secret.EncryptedText);
            }
        }

        [Fact]
        public async void BeDecryptable()
        {
            string secretId;

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");

                await context.EncryptSecretAsync();

                var decryptorType = Type.GetType(EncryptorMapping.ForwardMapping[context.Secret.EncryptionType]);
                var decryptor = _ctx.ServiceProvider.GetService(decryptorType) as ISymmetricEncryptor;
                
                Assert.Equal("test", decryptor?.Decrypt(context.Secret.EncryptedText, new List<string> { context.EncryptionKey }));
            }
        }
    }
}

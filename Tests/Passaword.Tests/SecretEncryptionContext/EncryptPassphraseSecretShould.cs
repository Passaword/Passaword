using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Passaword.Configuration;
using Passaword.Constants;
using Passaword.Encryption;
using Xunit;

namespace Passaword.Tests.SecretEncryptionContext
{
    public class EncryptPassphraseSecretShould
    {
        private readonly TestContext _ctx;

        public EncryptPassphraseSecretShould()
        {
            _ctx = new TestContext();
            _ctx.Configure();
            _ctx.Services.AddPassaword()
                .AddPassphraseValidation();
            _ctx.Build();
        }
        
        [Fact]
        public async void ChangeKeyWhenPassphraseIsPresent()
        {
            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                context.InputData.Add(UserInputConstants.Passphrase, "test");

                await context.EncryptSecretAsync();
                
                Assert.NotEqual(_ctx.Configuration["Passaword:DefaultKey"], context.EncryptionKey);
            }
        }

        [Fact]
        public async void NotBeDecryptableWithDefaultKey()
        {
            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                context.InputData.Add(UserInputConstants.Passphrase, "test");
                
                await context.EncryptSecretAsync();

                var decryptorType = Type.GetType(EncryptorMapping.ForwardMapping[context.Secret.EncryptionType]);
                var decryptor = _ctx.ServiceProvider.GetService(decryptorType) as ISymmetricEncryptor;

                Assert.Throws<CryptographicException>(()=>
                {
                    decryptor?.Decrypt(context.Secret.EncryptedText,
                            new List<string> {_ctx.Configuration["Passaword:DefaultKey"]});
                });
            }
        }

        [Fact]
        public async void BeDecryptable()
        {
            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                context.InputData.Add(UserInputConstants.Passphrase, "test");

                await context.EncryptSecretAsync();

                var decryptorType = Type.GetType(EncryptorMapping.ForwardMapping[context.Secret.EncryptionType]);
                var decryptor = _ctx.ServiceProvider.GetService(decryptorType) as ISymmetricEncryptor;

                Assert.Equal("test", decryptor?.Decrypt(context.Secret.EncryptedText, new List<string> { context.EncryptionKey }));
            }
        }
    }
}

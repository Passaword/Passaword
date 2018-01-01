using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Passaword.Constants;
using Passaword.Events;
using Xunit;

namespace Passaword.Tests.PassawordContext
{
    public class PassawordContextShould
    {
        private readonly TestContext _ctx;

        public PassawordContextShould()
        {
            _ctx = new TestContext();
            _ctx.ConfigureAndBuild();
        }

        [Fact]
        public async void RaiseOnEncryptedEvent()
        {
            var pc = _ctx.ServiceProvider.GetService<Passaword.PassawordContext>();
            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");

                await Assert.RaisesAsync<EncryptionEventArgs>(h => pc.OnSecretEncrypted += h, h => pc.OnSecretEncrypted -= h, async () =>
                {
                    await context.EncryptSecretAsync();
                });
            }
        }

        [Fact]
        public async void RaiseOnDecryptedEvent()
        {
            var pc = _ctx.ServiceProvider.GetService<Passaword.PassawordContext>();
            string secretId;
            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");

                secretId = await context.EncryptSecretAsync();
            }

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretDecryptionContext>())
            {
                await Assert.RaisesAsync<DecryptionEventArgs>(h => pc.OnSecretDecrypted += h, h => pc.OnSecretDecrypted -= h, async () =>
                {
                    await context.DecryptSecretAsync(secretId);
                });
            }
        }


        [Fact]
        public async void RaiseOnDecryptionFailedEvent()
        {
            var pc = _ctx.ServiceProvider.GetService<Passaword.PassawordContext>();
            string secretId;
            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");

                secretId = await context.EncryptSecretAsync();
            }

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretDecryptionContext>())
            {
                context.EncryptionKey = "test";
                context.DecryptionKeys.Clear();

                await Assert.RaisesAsync<DecryptionFailedEventArgs>(h => pc.OnDecryptionFailed += h, h => pc.OnDecryptionFailed -= h, async () =>
                {
                    await context.DecryptSecretAsync(secretId);
                });
            }
        }

        [Fact]
        public async void RaiseOnPreValidationFailedEvent()
        {
            var pc = _ctx.ServiceProvider.GetService<Passaword.PassawordContext>();
            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretEncryptionContext>())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");

                await context.EncryptSecretAsync();
            }

            using (var context = _ctx.ServiceProvider.GetService<Passaword.SecretDecryptionContext>())
            {
                await Assert.RaisesAsync<DecryptionFailedEventArgs>(h => pc.OnPreValidationFailed += h, h => pc.OnPreValidationFailed -= h, async () =>
                {
                    await context.PreProcessAsync("test");
                });
            }
        }
    }
}

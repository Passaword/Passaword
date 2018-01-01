using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Passaword.Configuration;
using Passaword.Constants;
using Passaword.Encryption;
using Xunit;

namespace Passaword.Tests.SecretDecryptionContext
{
    public class DecryptAuthenticatedSecretShould
    {
        private readonly TestContext _ctx;

        public DecryptAuthenticatedSecretShould()
        {
            _ctx = new TestContext();
            _ctx.Configure();
            _ctx.Services.AddPassaword()
                .AddUserEmailValidation();
            _ctx.Build();
        }
        
        [Fact]
        public async void NotBeDecryptableByAnotherUser()
        {
            string secretId;
            var contextService = _ctx.ServiceProvider.GetService<ISecretContextService>();
            using (var context = contextService.CreateEncryptionContext())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                context.InputData.Add(UserInputConstants.EmailAddress, "test@test.com");
                context.InputData.Add(UserInputConstants.ForceAuthentication, true);
                secretId = await context.EncryptSecretAsync();
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Email, "test@example.com")
            }, "test"));

            using (var context = contextService.CreateDecryptionContext(user))
            {
                Assert.Null(await context.DecryptSecretAsync(secretId));
            }
        }

        [Fact]
        public async void BeDecryptableByTargetUser()
        {
            string secretId;
            var contextService = _ctx.ServiceProvider.GetService<ISecretContextService>();
            using (var context = contextService.CreateEncryptionContext())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                context.InputData.Add(UserInputConstants.EmailAddress, "test@test.com");
                context.InputData.Add(UserInputConstants.ForceAuthentication, true);
                secretId = await context.EncryptSecretAsync();
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Email, "test@test.com")
            }, "test"));
            
            using (var context = contextService.CreateDecryptionContext(user))
            {
                Assert.Equal("test", await context.DecryptSecretAsync(secretId));
            }
        }
    }
}

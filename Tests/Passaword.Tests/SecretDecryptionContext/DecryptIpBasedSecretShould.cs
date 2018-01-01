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
    public class DecryptIpBasedSecretShould
    {
        private readonly TestContext _ctx;

        public DecryptIpBasedSecretShould()
        {
            _ctx = new TestContext();
            _ctx.Configure();
            _ctx.Services.AddPassaword()
                .AddUserIpValidation();
            _ctx.Build();
        }
        
        [Fact]
        public async void NotBeDecryptableByAnotherIp()
        {
            string secretId;
            var contextService = _ctx.ServiceProvider.GetService<ISecretContextService>();
            using (var context = contextService.CreateEncryptionContext())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                context.InputData.Add(UserInputConstants.IpRegex, "^127\\.0\\.0\\.1$");
                secretId = await context.EncryptSecretAsync();
            }
            
            using (var context = contextService.CreateDecryptionContext())
            {
                context.InputData.Add(UserInputConstants.IpAddress, "127.0.0.2");
                Assert.Null(await context.DecryptSecretAsync(secretId));
            }
        }

        [Fact]
        public async void BeDecryptableByTargetIp()
        {
            string secretId;
            var contextService = _ctx.ServiceProvider.GetService<ISecretContextService>();
            using (var context = contextService.CreateEncryptionContext())
            {
                context.InputData.Add(UserInputConstants.Secret, "test");
                context.InputData.Add(UserInputConstants.IpRegex, "^127\\.0\\.0\\.1$");
                secretId = await context.EncryptSecretAsync();
            }

            using (var context = contextService.CreateDecryptionContext())
            {
                context.InputData.Add(UserInputConstants.IpAddress, "127.0.0.1");
                Assert.Equal("test", await context.DecryptSecretAsync(secretId));
            }
        }
    }
}

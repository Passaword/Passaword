using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Passaword.Encryption;
using Xunit;

namespace Passaword.Tests.EncryptionTests.Aes256EncryptorTests
{
    public class Aes256EncryptorShould
    {
        private readonly ISymmetricEncryptor _encryptor;
        private TestContext _context = new TestContext();

        public Aes256EncryptorShould()
        {
            _context.ConfigureAndBuild();
            _encryptor = _context.ServiceProvider.GetService<ISymmetricEncryptor>();
        }

        [Fact]
        public void EncryptAndDecrypt()
        {
            var key = "8C662D023DD014B483AC66546573C9FA64185F1704E6B610F4AAA0DE30CE9016";
            string text = "this is my ßecrͼt teݨt";

            var encrypted = _encryptor.Encrypt(text, key);

            Assert.Equal(text, _encryptor.Decrypt(encrypted, key));
        }

    }
}

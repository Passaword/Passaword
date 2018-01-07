using System;
using System.Collections.Generic;
using System.Text;
using Passaword.Encryption;
using Xunit;

namespace Passaword.Tests.EncryptionTests.RsaEncryptorTests
{
    public class RsaEncryptorShould
    {
        private readonly RsaEncryptor _encryptor;

        public RsaEncryptorShould()
        {
            _encryptor = new RsaEncryptor();
        }

        [Fact]
        public void ReturnAKeyPair()
        {
            var param = _encryptor.GenerateKeyPair();
            Assert.NotNull(param.D);
        }

        [Fact]
        public void EncodeAndDecodeKeyPairCorrectly()
        {
            var param = _encryptor.GenerateKeyPair();

            var privateParams = _encryptor.EncodePrivateJsonParameters(param);
            var publicParams = _encryptor.EncodePublicJsonParameters(param);

            var decodedPrivateParams = _encryptor.DecodeJsonParameters(privateParams);
            var decodedPublicParams = _encryptor.DecodeJsonParameters(publicParams);

            Assert.Equal(param.D, decodedPrivateParams.D);
            Assert.Equal(param.DP, decodedPrivateParams.DP);
            Assert.Equal(param.DQ, decodedPrivateParams.DQ);
            Assert.Equal(param.InverseQ, decodedPrivateParams.InverseQ);
            Assert.Equal(param.P, decodedPrivateParams.P);
            Assert.Equal(param.Q, decodedPrivateParams.Q);
            Assert.Equal(param.Exponent, decodedPublicParams.Exponent);
            Assert.Equal(param.Modulus, decodedPublicParams.Modulus);
        }

        [Fact]
        public void EncryptAndDecrypt()
        {
            string text = "this is my ßecrͼt teݨt";
            var param = _encryptor.GenerateKeyPair();

            var param2 = _encryptor.GenerateKeyPair();

            param.Exponent = param2.Exponent;
            param.Modulus = param2.Modulus;

            var encrypted = _encryptor.EncryptToBase64(text, param);

            var decrypted = _encryptor.DecryptBase64(encrypted, param2);

            Assert.Equal(text, decrypted);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Passaword.Encryption
{
    public class RsaEncryptor
    {
        public RSAParameters GenerateKeyPair(int keySize = 4096)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.KeySize = keySize;

                return rsa.ExportParameters(true);
            }
        }

        public string EncryptToBase64(string input, RSAParameters parameters, Encoding encoding = null)
        {
            return Convert.ToBase64String(Encrypt(input, parameters, encoding));
        }

        public string DecryptBase64(string input, RSAParameters parameters, Encoding encoding = null)
        {
            return Decrypt(Convert.FromBase64String(input), parameters, encoding);
        }

        public byte[] Encrypt(string input, RSAParameters parameters, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;

            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(parameters);

                return rsa.Encrypt(encoding.GetBytes(input), RSAEncryptionPadding.OaepSHA256);
            }
        }

        public string Decrypt(byte[] input, RSAParameters parameters, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(parameters);

                return encoding.GetString(rsa.Decrypt(input, RSAEncryptionPadding.OaepSHA256));
            }
        }

        public RSAParameters DecodeJsonParameters(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            var parameters = JsonConvert.DeserializeObject<RsaStringParameters>(json, settings);
            return parameters.ToRsa();
        }

        public string EncodePublicJsonParameters(RSAParameters parameters)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            var stringParams = new RsaPublicParameters(parameters);
            return JsonConvert.SerializeObject(stringParams, settings);
        }

        public string EncodePrivateJsonParameters(RSAParameters parameters)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            var stringParams = new RsaPrivateParameters(parameters);
            return JsonConvert.SerializeObject(stringParams, settings);
        }
    }
}

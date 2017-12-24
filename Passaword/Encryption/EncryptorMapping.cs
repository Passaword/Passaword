using System;
using System.Collections.Generic;
using System.Text;

namespace Passaword.Encryption
{
    public class EncryptorMapping
    {
        public static Dictionary<string, string> ForwardMapping = new Dictionary<string, string> {
            { Aes256SecretEncryptor.Name, typeof(Aes256SecretEncryptor).AssemblyQualifiedName }
        };

        public static Dictionary<string, string> BackwardMapping = new Dictionary<string, string> {
            { typeof(Aes256SecretEncryptor).AssemblyQualifiedName, Aes256SecretEncryptor.Name }
        };
    }
}

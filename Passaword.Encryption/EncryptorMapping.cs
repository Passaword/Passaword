using System;
using System.Collections.Generic;
using System.Text;

namespace Passaword.Encryption
{
    public class EncryptorMapping
    {
        public static Dictionary<string, string> ForwardMapping = new Dictionary<string, string> {
            { Aes256Encryptor.Name, typeof(Aes256Encryptor).AssemblyQualifiedName }
        };

        public static Dictionary<string, string> BackwardMapping = new Dictionary<string, string> {
            { typeof(Aes256Encryptor).AssemblyQualifiedName, Aes256Encryptor.Name }
        };
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Passaword.Constants;
using Passaword.Events;

namespace Passaword.Configuration.Options
{
    public class PassawordOptions
    {
        public EventHandler<EncryptionEventArgs> OnSecretEncrypted { get; set; }
        
        public EventHandler<DecryptionEventArgs> OnSecretDecrypted { get; set; }
        
        public EventHandler<DecryptionFailedEventArgs> OnDecryptionFailed { get; set; }

        public EventHandler<DecryptionFailedEventArgs> OnPreValidationFailed { get; set; }
    }
}

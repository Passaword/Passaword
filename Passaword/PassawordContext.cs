using System;
using System.Collections.Generic;
using Passaword.Events;
using Passaword.Validation;

namespace Passaword
{
    public class PassawordContext
    {
        public IList<ISecretValidationRuleProcessor> SecretValidationRuleProcessors { get; set; } =
            new List<ISecretValidationRuleProcessor>();

        public event EventHandler<EncryptionEventArgs> SecretEncrypted;
        public EventHandler<EncryptionEventArgs> OnSecretEncrypted { get; set; }
        
        public event EventHandler<DecryptionEventArgs> SecretDecrypted;
        public EventHandler<DecryptionEventArgs> OnSecretDecrypted { get; set; }

        public event EventHandler<DecryptionFailedEventArgs> DecryptionFailed;
        public EventHandler<DecryptionFailedEventArgs> OnDecryptionFailed { get; set; }

        public event EventHandler<DecryptionFailedEventArgs> PreValidationFailed;
        public EventHandler<DecryptionFailedEventArgs> OnPreValidationFailed { get; set; }
    }
}

using System;
using Passaword.Validation;

namespace Passaword.Events
{
    public class DecryptionEventArgs : EventArgs
    {
        public DecryptionEventArgs(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public SecretDecryptionContext Context { get; set; }
        public ValidationResult ValidationResult { get; set; }
    }
}

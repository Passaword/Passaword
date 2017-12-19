using System;

namespace Passaword.Events
{
    public class DecryptionFailedEventArgs : EventArgs
    {
        public DecryptionFailedEventArgs(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public SecretDecryptionContext Context { get; set; }
        public string FailureReason { get; set; }
        public Exception Exception { get; set; }
    }
}

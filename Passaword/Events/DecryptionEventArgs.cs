using System;

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
    }
}

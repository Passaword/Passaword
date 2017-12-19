using System;

namespace Passaword.Events
{
    public class EncryptionEventArgs : EventArgs
    {
        public EncryptionEventArgs(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get;}
        public SecretEncryptionContext Context { get; set; }
    }
}

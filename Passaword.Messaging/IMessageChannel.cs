using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passaword.Messaging
{
    public interface IMessageChannel
    {
        IMessage FormatMessage(string content, IDictionary<string, object> extraData);
        Task SendAsync(IMessage message);
    }
}

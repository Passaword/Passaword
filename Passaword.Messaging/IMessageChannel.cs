using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passaword.Messaging
{
    public interface IMessageChannel
    {
        IMessage GetMessage(string content, IDictionary<string, object> extraData);
        Task<string> FormatMessage(string messageType, IDictionary<string, string> data);
        Task SendAsync(IMessage message);
    }
}

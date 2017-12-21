using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passaword.Messaging
{
    public interface IMessageContentStore
    {
        Task<string> GetTemplate(string messageType);
        string ReplaceTags(string content, IDictionary<string, string> data);
    }
}

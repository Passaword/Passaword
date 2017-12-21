using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Passaword.Messaging
{
    public class DefaultMessageContentStore : IMessageContentStore
    {
        private readonly IConfiguration _config;

        public DefaultMessageContentStore(IConfiguration config)
        {
            _config = config;
        }


        public Task<string> GetTemplate(string messageType)
        {
            return Task.FromResult(_config["Passaword:Messaging:Content:" + messageType]);
        }

        public string ReplaceTags(string content, IDictionary<string, string> data)
        {
            if (content == null) return null;
            foreach (var kv in data)
            {
                content = content.Replace($"{{{kv.Key}}}", kv.Value);
            }
            return content;
        }
    }
}

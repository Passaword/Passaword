using System;
using System.Collections.Generic;
using System.Text;

namespace Passaword.Messaging.Email
{
    public interface IEmailConfiguration
    {
        string SmtpServer { get; set; }
        int SmtpPort { get; set; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
        bool UseSsl { get; set; }
        bool RequiresAuthentication { get; set; }
        string DefaultFromAddress { get; set; }
        string DefaultFromName { get; set; }
    }

    public class EmailConfiguration : IEmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool UseSsl { get; set; } = true;
        public bool RequiresAuthentication { get; set; } = true;
        public string DefaultFromAddress { get; set; }
        public string DefaultFromName { get; set; }
    }
}

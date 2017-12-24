using System;
using System.Collections.Generic;
using System.Text;
using MimeKit.Text;
using Passaword.Constants;

namespace Passaword.Messaging.Email
{
    public class EmailMessage : IMessage
    {
        public EmailMessage() { }

        public EmailMessage(EmailAddress to)
        {
            To.Add(to);
        }

        public EmailMessage(EmailAddress from, EmailAddress to)
        {
            From.Add(from);
            To.Add(to);
        }

        public string Subject { get; set; }
        public EmailConstants.TextFormats TextFormat { get; set; } = EmailConstants.TextFormats.PlainText;
        public string Content { get; set; }
        public List<EmailAddress> From { get; set; } = new List<EmailAddress>();
        public List<EmailAddress> To { get; set; } = new List<EmailAddress>();
        public EmailAddress ReplyTo { get; set; }
    }
}

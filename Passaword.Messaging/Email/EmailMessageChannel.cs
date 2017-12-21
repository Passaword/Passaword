using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Passaword.Constants;

namespace Passaword.Messaging.Email
{
    public interface IEmailMessageChannel : IMessageChannel
    {
        Task SendAsync(EmailMessage message);
    }

    public class EmailMessageChannel : IEmailMessageChannel
    {
        private readonly IEmailConfiguration _config;
        private readonly IMessageContentStore _contentStore;

        public EmailMessageChannel(IEmailConfiguration config, IMessageContentStore contentStore)
        {
            _config = config;
            _contentStore = contentStore;
        }
        
        public IMessage GetMessage(string content, IDictionary<string, object> extraData)
        {
            EmailMessage message = new EmailMessage
            {
                Content = content,
                Subject = extraData[EmailConstants.Subject].ToString(),
                To = (List<EmailAddress>) extraData[EmailConstants.To],
                From = (List<EmailAddress>) extraData[EmailConstants.From]
            };

            if (extraData.ContainsKey(EmailConstants.TextFormat))
            {
                message.TextFormat = (EmailConstants.TextFormats) extraData[EmailConstants.TextFormat];
            }
            else
            {
                message.TextFormat = EmailConstants.TextFormats.PlainText;
            }

            if (extraData.ContainsKey(EmailConstants.ReplyTo))
            {
                message.ReplyTo = (EmailAddress) extraData[EmailConstants.ReplyTo];
            }
            return message;
        }

        public async Task<string> FormatMessage(string messageType, IDictionary<string, string> data)
        { 
            string content = await _contentStore.GetTemplate(messageType);

            return _contentStore.ReplaceTags(content, data);
        }

        private TextFormat GetFormat(EmailConstants.TextFormats format)
        {
            switch (format)
            {
                case EmailConstants.TextFormats.Html:
                    return TextFormat.Html;
                case EmailConstants.TextFormats.PlainText:
                default:
                    return TextFormat.Plain;
            }
        }

        public async Task SendAsync(IMessage message)
        {
            var emailMessage = (EmailMessage) message;
            await SendAsync(emailMessage);
        }

        public async Task SendAsync(EmailMessage emailMessage)
        {
            var email = new MimeMessage();

            email.To.AddRange(emailMessage.To.Select(x => new MailboxAddress(x.Name, x.Address)));
            if (emailMessage.From.Count > 0)
            {
                email.From.AddRange(emailMessage.From.Select(x => new MailboxAddress(x.Name, x.Address)));
            }
            else
            {
                email.From.Add(new MailboxAddress(_config.DefaultFromName, _config.DefaultFromAddress));
            }

            email.Subject = emailMessage.Subject;
            if (emailMessage.ReplyTo != null)
            {
                email.ReplyTo.Add(new MailboxAddress(emailMessage.ReplyTo.Name, emailMessage.ReplyTo.Address));
            }
            email.Body = new TextPart(GetFormat(emailMessage.TextFormat))
            {
                Text = emailMessage.Content
            };

            var emailClient = new SmtpClient();
            emailClient.Connect(_config.SmtpServer, _config.SmtpPort, _config.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

            if (_config.RequiresAuthentication)
            {
                emailClient.Authenticate(_config.SmtpUsername, _config.SmtpPassword);
            }

            await emailClient.SendAsync(email);

            emailClient.Disconnect(true);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Passaword.Configuration.Options;
using Passaword.Constants;
using Passaword.Encryption;
using Passaword.Encryption.KeyGen;
using Passaword.Events;
using Passaword.Messaging;
using Passaword.Messaging.Email;
using Passaword.Storage;
using Passaword.Validation;
using Passaword.Validation.Expiry;
using Passaword.Validation.Passphrase;
using Passaword.Validation.UserEmail;
using Passaword.Validation.UserIp;

namespace Passaword.Configuration
{
    public static class PassawordServiceCollectionExtensions
    {
        public static IPassawordBuilder AddPassaword(this IServiceCollection services, Action<PassawordOptions> options = null)
        {
            var context = new PassawordContext();

            var o = new PassawordOptions();
            options?.Invoke(o);

            context.OnDecryptionFailed = o.OnDecryptionFailed;
            context.OnSecretDecrypted = o.OnSecretDecrypted;
            context.OnSecretEncrypted = o.OnSecretEncrypted;
            context.OnPreValidationFailed = o.OnPreValidationFailed;

            services.AddSingleton<PassawordContext>(context);
            services.AddTransient<ISecretValidator, DefaultSecretValidator>();
            services.AddTransient<IKeyGenerator, DefaultKeyGenerator>();
            services.AddTransient<ISymmetricEncryptor, Aes256Encryptor>();
            services.AddTransient<Aes256Encryptor, Aes256Encryptor>();
            services.AddTransient<SecretEncryptionContext, SecretEncryptionContext>();
            services.AddTransient<SecretDecryptionContext, SecretDecryptionContext>();
            services.AddTransient<EncryptionEventArgs, EncryptionEventArgs>();
            services.AddTransient<DecryptionEventArgs, DecryptionEventArgs>();
            services.AddTransient<DecryptionFailedEventArgs, DecryptionFailedEventArgs>();
            services.AddTransient<ISecretContextService, DefaultSecretContextService>();
            services.AddTransient<IMessageContentStore, DefaultMessageContentStore>();

            return new PassawordBuilder()
            {
                Services = services
            };
        }

        public static IPassawordBuilder AddInMemorySecretStore(this IPassawordBuilder pb)
        {
            pb.Services.AddSingleton<ISecretStore, InMemorySecretStore>();
            return pb;
        }

        public static IPassawordBuilder AddPassphraseValidation(this IPassawordBuilder pb, Action<PassphraseValidatorOptions> options = null)
        {
            pb.Services.AddSingleton<PassphraseValidationRuleProcessor, PassphraseValidationRuleProcessor>();
            var serviceProvider = pb.Services.BuildServiceProvider();
            
            var passphrase = serviceProvider.GetService<PassphraseValidationRuleProcessor>();
            var context = serviceProvider.GetService<PassawordContext>();
            var o = new PassphraseValidatorOptions();
            options?.Invoke(o);

            passphrase.IsRequired = o.IsRequired;
            passphrase.ValidationStage = o.ValidationStage;

            context.SecretValidationRuleProcessors.Add(passphrase);
            return pb;
        }

        public static IPassawordBuilder AddExpiryValidation(this IPassawordBuilder pb, Action<ExpiryValidatorOptions> options = null)
        {
            pb.Services.AddTransient<ExpiryValidationRuleProcessor, ExpiryValidationRuleProcessor>();
            var serviceProvider = pb.Services.BuildServiceProvider();
            var expiry = serviceProvider.GetService<ExpiryValidationRuleProcessor>();
            var context = serviceProvider.GetService<PassawordContext>();

            var o = new ExpiryValidatorOptions();
            options?.Invoke(o);

            expiry.IsRequired = o.IsRequired;
            expiry.ValidationStage = o.ValidationStage;

            expiry.ValidationStage = Constants.ValidationStage.AfterGet;
            context.SecretValidationRuleProcessors.Add(expiry);
            return pb;
        }

        public static IPassawordBuilder AddUserIpValidation(this IPassawordBuilder pb, Action<UserIpValidatorOptions> options = null)
        {
            pb.Services.AddTransient<UserIpValidationRuleProcessor, UserIpValidationRuleProcessor>();
            var serviceProvider = pb.Services.BuildServiceProvider();
            var userIp = serviceProvider.GetService<UserIpValidationRuleProcessor>();
            var context = serviceProvider.GetService<PassawordContext>();

            var o = new UserIpValidatorOptions();
            options?.Invoke(o);

            userIp.IsRequired = o.IsRequired;
            userIp.ValidationStage = o.ValidationStage;
            
            context.SecretValidationRuleProcessors.Add(userIp);
            return pb;
        }

        public static IPassawordBuilder AddUserEmailValidation(this IPassawordBuilder pb, Action<UserEmailValidatorOptions> options = null)
        {
            pb.Services.AddTransient<UserEmailValidationRuleProcessor, UserEmailValidationRuleProcessor>();
            var serviceProvider = pb.Services.BuildServiceProvider();
            var userEmail = serviceProvider.GetService<UserEmailValidationRuleProcessor>();
            var context = serviceProvider.GetService<PassawordContext>();

            var o = new UserEmailValidatorOptions();
            options?.Invoke(o);

            userEmail.IsRequired = o.IsRequired;
            userEmail.ValidationStage = o.ValidationStage;

            userEmail.ValidationStage = Constants.ValidationStage.AfterGet;
            context.SecretValidationRuleProcessors.Add(userEmail);
            return pb;
        }

        public static IPassawordBuilder AddEmailMessaging(this IPassawordBuilder pb, Action<EmailMessagingOptions> options = null)
        {
            var serviceProvider = pb.Services.BuildServiceProvider();
            var config = serviceProvider.GetService<IConfiguration>();
            pb.Services.AddSingleton<IEmailConfiguration>(config.GetSection("Passaword:EmailConfiguration").Get<EmailConfiguration>());
            pb.Services.AddTransient<IEmailMessageChannel, EmailMessageChannel>();

            var o = new EmailMessagingOptions();
            options?.Invoke(o);

            var context = serviceProvider.GetService<PassawordContext>();

            context.OnSecretEncrypted += async (ctx, e) =>
            {
                if (e.Context.GetInput(UserInputConstants.EmailAddress) != null && !e.Context.GetInput<bool>(UserInputConstants.DoNotSendEmail))
                {
                    var emailService = e.ServiceProvider.GetService<IEmailMessageChannel>();
                    var url = config["Passaword:SecretUrl"].Replace("{key}", HttpUtility.UrlEncode(e.Context.Secret.Id));
                    var custommessage = e.Context.GetInput(UserInputConstants.CustomMessage);
                    await emailService.SendAsync(
                        new EmailMessage(to: new EmailAddress(e.Context.GetInput(UserInputConstants.EmailAddress)))
                        {
                            Subject = config["Passaword:EmailConfiguration:EncryptSubject"],
                            Content = await emailService.FormatMessage(EmailConstants.MessageTypes.Encrypted, new Dictionary<string,string>
                            {
                                { "url", url },
                                { "custommessage", !string.IsNullOrEmpty(custommessage) ? custommessage + Environment.NewLine + Environment.NewLine : "" }
                            })
                        });
                }
            };

            if (o.SendOwnerEmailOnDecrypt)
            {
                context.OnSecretDecrypted += async (ctx, e) =>
                {
                    var email = e.Context.Secret.SecretProperties.FirstOrDefault(q =>
                        q.Type == SecretProperties.OwnerEmail);
                    if (email != null)
                    {
                        var emailService = e.ServiceProvider.GetService<IEmailMessageChannel>();
                        await emailService.SendAsync(
                            new EmailMessage(to: new EmailAddress(email.Data))
                            {
                                Subject = config["Passaword:EmailConfiguration:DecryptSubject"],
                                Content = await emailService.FormatMessage(EmailConstants.MessageTypes.Decrypted,
                                    new Dictionary<string, string>
                                    {
                                        {"secret", e.Context.Secret.Id}
                                    })
                            });
                    }
                };
            }

            return pb;
        }
    }
}

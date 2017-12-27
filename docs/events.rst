Events
=====================================

Events are registered against the global ``PassawordContext`` for common actions.

You can hook into these by getting a reference to the context in DI::

	public MyClass(PassawordContext ctx) {
		ctx.OnSecretEncrypted += (encryptionContext, eventArgs) {
			Console.WriteLine("Hey cool, your secret was encrypted!");
		};
	}

The events currently available are:

* ``OnSecretEncrypted``
* ``OnSecretDecrypted``
* ``OnPreValidationFailed``
* ``OnDecryptionFailed``

The email messaging system makes use of these to send emails. For example see the ``AddEmailMessaging`` source::

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
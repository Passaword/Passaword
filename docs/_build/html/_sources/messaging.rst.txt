Messaging
=====================================

A simple messaging API is provided to allow configuration of different providers. Out the box, and email messaging channel is provided, utilising ``MailKit``. 

This assumes you have an SMTP enabled mail server.

By default, plain text emails are sent to a provided email address on encryption, and can be sent back to an owner email address on decryption.

==================
Initialization
==================

Add this line to your ``Startup.cs`` file::

	.AddEmailMessaging(options =>
	{
		options.SendOwnerEmailOnDecrypt = true;
	})

Add the email settings to your appsettings.json config file::

	"Passaword": {
		"EmailConfiguration": {
		  "EncryptSubject": "You have been sent a secret",
		  "DecryptSubject": "Your secret has been received",
		  "SmtpServer": "smtp.example.com",
		  "SmtpPort": 587,
		  "SmtpUsername": "myemail@example.com",
		  "SmtpPassword": "mypassword",
		  "RequiresAuthentication": true,
		  "UseSsl": true,
		  "DefaultFromAddress": "myemail@example.com",
		  "DefaultFromName": "Passaword"
		},
		"Messaging": {
		  "Content": {
			"Encrypted": "{custommessage}You have been sent a secret. Use your passphrase to decrypt it (if you were given one) using the link below.\r\n\r\n{url}",
			"Decrypted": "The secret {secret} you sent has been received and destroyed"
		  }
		}
	  }

================
On Encrypt
================

When encrypting, pass the following data::

	encryptContext.InputData.Add(UserInputConstants.CustomMessage, customMessage);
	encryptContext.InputData.Add(UserInputConstants.EmailAddress, targetEmail);
	encryptContext.SecretProperties.Add(new SecretProperty(SecretProperties.OwnerEmail) { Data = ownerEmail });

If you want to suppress the email on encrypt, you can pass in ``encryptContext.InputData.Add(UserInputConstants.DoNotSendEmail, true);``

==============
Customisation
==============

It is possible to extend the messaging functionality with the following interfaces:

* ``IMessageContentStore`` - responsible for sourcing and formatting the message content. The ``DefaultContentStore`` does this using the appsettings.json file.
* ``IMessageChannel`` - defines a messaging channel
* ``IMessage`` - defines a message to send through an ``IMessageChannel``
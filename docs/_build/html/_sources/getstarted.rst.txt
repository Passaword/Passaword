Getting Started
=====================================

===============================
Installation
===============================

The package is available on NuGet:

``Install-Package Passaword``

If you want to run a web version of Passaword, a good place to start is the sample UI, available at https://github.com/Passaword/Passaword.UI

This gives you a really simple UI with Google authentication enabled.

===============================
Dependency Injection
===============================

The first step is to initialize the services Passaword needs according to your desired configuration.

In your ``Startup.cs`` file, add the following line inside ``ConfigureServices``::

    services.AddPassaword()
		.AddInMemorySecretStore()
		.AddExpiryValidation()
		.AddPassphraseValidation();

This sets up all the services you need for a basic installation with expiry and passphrase validation.

===============
Configuration
===============

Ensure you add  your configuration defaults in ``appsettings.json`` (secure secrets in your secure User Secrets storage)::

    "Passaword": {
	"DefaultKey": "25EC2A10DCFE73790A81D02388C000DACD4B6D4DD8603401EA0BF0CBA40DB1E6",
	"DecryptionKeys": [],
	"SecretUrl": "https://passaword.mydomain.com/secret?k={key}"
	}

**Please generate your OWN key**

---------------------
Key rotation
---------------------

The decryption keys array is there so you can rotate the default key. Whenever you set a new encryption key, add the old one to the array.

===============================
Encrypting a secret
===============================

To encrypt a secret, you need a ``SecretEncryptionContext``, provided by an ``ISecretContextService``, which you can get via DI.

Within the encryption context, you should pass in any data the validators need, and then call the ``EncryptSecretAsync`` method::

	string secretId;
    using (var encryptContext = _secretContextService.CreateEncryptionContext(User))
	{
		encryptContext.Secret.CreatedBy = User.FindFirstValue(ClaimTypes.Email);
		encryptContext.InputData.Add(UserInputConstants.Secret, model.Secret);
		encryptContext.InputData.Add(UserInputConstants.Passphrase, model.Passphrase);
		encryptContext.InputData.Add(UserInputConstants.Expiry, model.Expiry);
		encryptContext.SecretProperties.Add(new SecretProperty(SecretProperties.OwnerEmail) { Data = User.FindFirstValue(ClaimTypes.Email) });

		secretId = await encryptContext.EncryptSecretAsync();
	}

That's all. Remember to dispose of the context so your secret doesn't hang around in memory.

===============================
Decrypting a secret
===============================

Decrypting a secret is a 2-step process, a pre-process step to check the user supplied key, and then the actual decryption step. This is condensed in to one call for brevity::

    using (var decryptContext = _secretContextService.CreateDecryptionContext(User))
	{
		decryptContext.InputData.Add(UserInputConstants.Passphrase, model.Passphrase);

		var result = await decryptContext.PreProcessAsync(model.Id);
		if (result.IsValid)
		{
			var decrypted = await decryptContext.DecryptSecretAsync(model.Id);
			if (decrypted == null) {
			    throw new Exception("Decryption failed");
			}
			return decrypted;
		}
		else
		{
		    throw new Exception("Decryption failed");
		}
	}

That's it! You can now encrypt and decrypt a secret.
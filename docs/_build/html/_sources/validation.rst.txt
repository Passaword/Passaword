Validation
=====================================

Before a secret is decrpyted, it can be passed through a series of validation/verification criteria to make sure it is being decrypted by the right person.

Validation can happen at different stages of decryption, once immediately after the secret is retrieved from the store and once immediately before it is decrypted.

Out of the box, several validation criteria are available for your use:

* Expiry: Checks that the secret has not expired
* User Email: Checks that the logged in user's email matches that which the secret was sent to
* User IP: Checks that the user's IP matches a supplied regular expression
* Passphrase: Uses a user supplied passphrase to change the decryption key before decrypting

Of course, if you are not happy with the default validation methods or want to add your own, you can bend the system to your will with the DI container.

=======================
Expiry
=======================

Expiry validation ensures a secret cannot be decrypted after a certain date. By default this runs immediately after getting the secret from the store.

Add the following to your Passaword initialization code::

	services.AddPassaword()
		.AddExpiryValidation();

Within the encryption context, ensure you add the expiry of the secret with a key of ``UserInputConstants.Expiry``.

=======================
User Email
=======================

User email validation ensures a secret cannot be decrypted by a user other than the one who was sent the email. By default this runs immediately after getting the secret from the store.

Add the following to your Passaword initialization code::

	services.AddPassaword()
		.AddUserEmailValidation();

Within the encryption context, ensure you add the email of the target user with a key of ``UserInputConstants.EmailAddress`` and set the following: ``encryptContext.InputData.Add(UserInputConstants.ForceAuthentication, true)``.

=======================
User IP
=======================

User IP validation ensures a secret cannot be decrypted by a user who's IP does not match a given regular expression. By default this runs immediately after getting the secret from the store.

Add the following to your Passaword initialization code::

	services.AddPassaword()
		.AddUserIpValidation();

Within the encryption context, ensure you add the regex of the IP for a valid user with a key of ``UserInputConstants.IpRegex``.

Within the decryption context, ensure you add the IP address of the user with a key of ``UserInputConstants.IpAddress``. There is an extension built in to Passaword.Utils to help with this: ``Request.GetIpAddress()``.

=======================
Passphrase
=======================

Passphrase validation is not strictly a validator, but actually transforms the encryption key to one derived by PBKDF2 using a user-supplied passphrase. By default this runs immediately before decrypting the secret.

Add the following to your Passaword initialization code::

	services.AddPassaword()
		.AddPassphraseValidation();

Within the encryption context, ensure you add the passphrase with a key of ``UserInputConstants.Passphrase``.

Within the decryption context, ensure you add the passphrase with a key of ``UserInputConstants.Passphrase``. Clearly for this to be of any use, the passphrase in the decryption context should be supplied by the user attempting to decrypt the secret.

=======================
Roll your own
=======================

You can validate secrets however you like. How about validating that the user only decrypts at a very specific date and time?

To roll your own validation, you need to implement ``ISecretValidationRuleProcessor``. It may be easier to inherit from ``BaseValidationRuleProcessor``.

Firstly, override the ``Name`` property with the name of your validator. It should be unique among other validators you are using.

There are 2 main methods to override::

	public override void CreateRule(SecretEncryptionContext encryptionContext, ClaimsPrincipal principal)
    {
        throw new NotImplementedException("Rule setup not implemented");
    }

    public override ValidationResult Validate(SecretDecryptionContext secret, string validationData, ClaimsPrincipal principal)
    {
        throw new NotImplementedException("Rule validation not implemented");
    }

---------------
Encrypting
---------------

The first, ``CreateRule`` is the rule that is run when encrypting a secret. You should use this to store any information you need to run the validation later on. You can do this like so::

	encryptionContext.AddValidationRule(new SecretValidationRule
    {
        Validator = this.Name,
        ValidationData = SerializeData(yourOwnDataObject)
    });

FYI, ``SerializeData`` is a simple JSON serializer implemented in the base class. You can override this if necessary.

---------------
Decrypting
---------------

When it comes to decrypting your secret, you should deserialize your data and perform whatever checks necessary to return a ValidationResult.

An example from the expiry validator is shown::

	var expiryData = DeserializeData<ExpiryValidationData>(validationData);

    var isValid = !expiryData.Expiry.HasValue ||  DateTime.Now < expiryData.Expiry;
    _logger.LogDebug($"Validating expiry: {isValid}");
    if (isValid)
    {
        return ValidationResult.SuccessResult;
    }
    else
    {
        return new ValidationResult(false)
        {
            Error = "Secret expired",
            ValidationPointOfFailure = this.Name
        };
    }

---------------
Initialization
---------------

To get your validator into the workflow, you should add it to the global ``PassawordContext``.

Here's how, in your ``ConfigureServices`` section (although it's best practice to move this out into an extension method on top of ``IPassawordBuilder``).::

	services.AddTransient<MyValidationRuleProcessor, MyValidationRuleProcessor>(); //add your validator to DI
	var serviceProvider = services.BuildServiceProvider(); //build the service provider
	var myValidator = serviceProvider.GetService<MyValidationRuleProcessor>(); //get a new reference to your validator
	myValidator.ValidationStage = Constants.ValidationStage.AfterGet; // set up the pipeline
	var context = serviceProvider.GetService<PassawordContext>(); //get the global Passaword Context

	context.SecretValidationRuleProcessors.Add(myValidator); //add the validator to the list


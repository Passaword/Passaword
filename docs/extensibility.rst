Extensibility
=====================================

Passaword is highly configurable and can be overridden / replaced at various levels to achieve your desired business logic. If you don't like how the default implementations work, write your own!

In most cases, the default implementations will be named 'Default' plus the interface name. e.g. ``DefaultSecretContextService``.

=======================
Core
=======================

* ``ISecretContextService`` - sets up the encryption and decryption contexts
* ``SecretEncryptionContext`` - orchestrates the full encryption process - it is possible to override most methods in this class
* ``SecretDecryptionContext`` - orchestrates the full decryption process - it is possible to override most methods in this class
* ``IKeyGenerator`` - generates keys, salts and returns the stored default key and decryption keys
* ``ISecretEncryptor`` - encrypts a secret given a key and decrypts it given a list of keys - default implementation: ``Aes256SecretEncryptor``. **Please note**: you should be careful to add any new implementations of this interface into ``EncryptorMapping`` on startup, which is responsible for retrieving the correct class from a string stored against each secret.
* ``ISecretValidationRuleProcessor`` - executes custom validation logic against a stored secret when decrypting
* ``ISecretValidator`` - orchestrates the validation logic within a decryption context

=======================
Storage
=======================

* ``ISecretStore`` - responsible for CRUD operations for secrets

=======================
Messaging
=======================

* ``IMessageContentStore`` - responsible for sourcing and formatting the message content. The ``DefaultContentStore`` does this using the appsettings.json file.
* ``IMessageChannel`` - defines a messaging channel
* ``IMessage`` - defines a message to send through an ``IMessageChannel``


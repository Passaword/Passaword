Security
=====================================

===============================
Out the box
===============================

The following security features are provided out the box:

* Secrets are encrypted using AES 256
* Secrets can be set to expire after any amount of time
* Encryption key can be derived from a user supplied passphrase (key derivation by PBKDF2)
* Decryption can be restricted to users based on their logged-in email address
* Decryption can be restricted by IP

The system is highly flexible and configurable, so you can define your own validation criteria or override the out of the box functionality.

One example could be to delete a secret after X failed decryptions.

===============================
Can someone intercept a secret?
===============================

Yes - although unlikely, that's why you need to use a layered approach to make it as difficult as possible. It's recommended to always use a passphrase and supply that via a different channel.

In the worst case where someone has decrypted your secret, you will know about it due to the self-destructing nature of Passaword secrets. You should reset that secret / password and resend it.

==================
Server Security
==================

It is out of scope for the package to secure your server. It is assumed the environment you install the package into is secure.

If you are running a web server, that means ensuring you use TLS and secure headers.
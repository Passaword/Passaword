using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Passaword.Configuration;
using Passaword.Constants;
using SysConsole = System.Console;

namespace Passaword.Tests.Console
{
    class Program
    {
        public static IServiceProvider Services;

        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();


            var services = new ServiceCollection();
            services.AddLogging();
            services.AddOptions();
            services.AddSingleton<IConfiguration>(configuration);

            services.AddPassaword()
                .AddInMemorySecretStore()
                .AddExpiryValidation()
                .AddPassphraseValidation();
            Services = services.BuildServiceProvider();

            SysConsole.WriteLine("Welcome to the Passaword console. Enter a command to start, or type help if you are not sure where to start.");
            SysConsole.WriteLine("");

            while (true)
            {
                string[] command = SysConsole.ReadLine().Split(' ');

                if (command.First() == "exit")
                {
                    break;
                }

                try
                {
                    switch (command.First())
                    {

                        case "encrypt":
                            SysConsole.WriteLine(Encrypt(command).Result);
                            break;
                        case "decrypt":
                            SysConsole.WriteLine(Decrypt(command).Result);
                            break;
                        case "help":
                            Help();
                            break;
                    }
                }
                catch(Exception e)
                {
                    SysConsole.WriteLine($"Command failed: {e.Message}");
                }
            }
        }

        static async Task<string> Encrypt(string[] input)
        {
            var secretContextService = Services.GetService<ISecretContextService>();

            if (input.Length == 1)
            {
                return "You need to enter the secret";
            }

            string secretId;
            using (var encryptContext = secretContextService.CreateEncryptionContext(new ClaimsPrincipal()))
            {
                encryptContext.InputData.Add(UserInputConstants.Secret, input[1]);
                if (input.Length > 2)
                {
                    encryptContext.InputData.Add(UserInputConstants.Passphrase, input[2]);
                }
                encryptContext.InputData.Add(UserInputConstants.Expiry, DateTime.Now.AddDays(7));

                secretId = await encryptContext.EncryptSecretAsync();
            }

            return $"Encrypted secret to {secretId}";
        }

        static async Task<string> Decrypt(string[] input)
        {
            var secretContextService = Services.GetService<ISecretContextService>();

            if (input.Length == 1)
            {
                return "You need to enter the secret ID";
            }

            using (var decryptContext = secretContextService.CreateDecryptionContext(new ClaimsPrincipal()))
            {
                if (input.Length > 2)
                {
                    decryptContext.InputData.Add(UserInputConstants.Passphrase, input[2]);
                }

                var result = await decryptContext.PreProcessAsync(input[1]);
                if (result.IsValid)
                {
                    var decrypted = await decryptContext.DecryptSecretAsync(input[1]);
                    if (decrypted == null)
                    {
                        return "Decryption failed";
                    }
                    return $"Decryption success: {decrypted}";
                }
                else
                {
                    return "Decryption failed";
                }
            }
        }

        static void Help()
        {
            SysConsole.WriteLine("encrypt <secret> <passphrase (optional)> - Encrypt a secret");
            SysConsole.WriteLine("decrypt <secret ID> <passphrase (optional)> - Decrypt a secret");
            SysConsole.WriteLine("exit - Exit");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Passaword.Configuration;

namespace Passaword.Tests
{
    public class TestContext
    {
        public IConfiguration Configuration { get; set; }
        public IServiceCollection Services { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        public void ConfigureAndBuild(Dictionary<string, string> additionalConfiguration = null)
        {
            Configure(additionalConfiguration);
            Build();
        }

        public void Configure(Dictionary<string, string> additionalConfiguration = null)
        {
            if (additionalConfiguration != null)
            {
                foreach (var kv in additionalConfiguration)
                {
                    if (_defaultConfiguration.ContainsKey(kv.Key)) _defaultConfiguration[kv.Key] = kv.Value;
                    else _defaultConfiguration.Add(kv.Key, kv.Value);
                }
            }
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_defaultConfiguration)
                .Build();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddOptions();
            services.AddSingleton<IConfiguration>(configuration);

            services.AddPassaword()
                .AddInMemorySecretStore();


            Services = services;
        }

        public void Build()
        {
            ServiceProvider = Services.BuildServiceProvider();
            Configuration = ServiceProvider.GetService<IConfiguration>();
        }

        private static readonly Dictionary<string, string> _defaultConfiguration = new Dictionary<string, string>
        {
            { "Passaword:DefaultKey", "25EC2A10DCFE73790A81D02388C000DACD4B6D4DD8603401EA0BF0CBA40DB1E6" },
            { "Passaword:DecryptionKeys:0", "" },
            { "Passaword:SecretUrl", "https://passaword.mydomain.com/secret?k={key}" }
        };
    }
}

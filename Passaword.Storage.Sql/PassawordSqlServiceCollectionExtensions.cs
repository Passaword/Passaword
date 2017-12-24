using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Passaword.Configuration;
using Passaword.Configuration.Options;
using Passaword.Validation.Expiry;

namespace Passaword.Storage.Sql
{
    public static class PassawordSqlServiceCollectionExtensions
    {
        public static IPassawordBuilder AddSqlSecretStore(this IPassawordBuilder pb, Action<ExpiryValidatorOptions> options = null)
        {
            var serviceProvider = pb.Services.BuildServiceProvider();
            var config = serviceProvider.GetService<IConfiguration>();

            pb.Services.AddTransient<ISecretStore, SqlSecretStore>();


            pb.Services.AddDbContext<PassawordDbContext>(q => q.UseSqlServer(config.GetConnectionString("Passaword")));
            return pb;
        }
    }
}

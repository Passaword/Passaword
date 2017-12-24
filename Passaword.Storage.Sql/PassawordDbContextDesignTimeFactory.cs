using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Passaword.Storage.Sql
{
    public class PassawordDbContextDesignTimeFactory : IDesignTimeDbContextFactory<PassawordDbContext>
    {

        public PassawordDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PassawordDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Passaword"));

            return new PassawordDbContext(optionsBuilder.Options);
        }
    }
}

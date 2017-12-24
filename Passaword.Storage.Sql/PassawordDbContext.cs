using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Passaword.Validation;

namespace Passaword.Storage.Sql
{
    public class PassawordDbContext : DbContext
    {
        public PassawordDbContext(DbContextOptions<PassawordDbContext> options) : base(options)
        {
            
        }

        public DbSet<Secret> Secrets { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SecretValidationRule>().Property(q => q.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Secret>().ToTable("Secret");
            modelBuilder.Entity<Secret>().HasIndex(q => q.Id).IsUnique();
            modelBuilder.Entity<Secret>().HasIndex(q => q.CreatedBy);
            modelBuilder.Entity<Secret>().HasIndex(q => q.CreatedByProvider);
            
            modelBuilder.Entity<Secret>().HasKey(q => q.Id);
            modelBuilder.Entity<SecretProperty>().HasKey(q=>new{ q.SecretId, q.Type }).ForSqlServerIsClustered();
            modelBuilder.Entity<SecretValidationRule>().HasKey(q => q.Id);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}

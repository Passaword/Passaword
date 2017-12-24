﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Passaword.Storage.Sql;
using System;

namespace Passaword.Storage.Sql.Migrations
{
    [DbContext(typeof(PassawordDbContext))]
    [Migration("20171224114001_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Passaword.Secret", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(32);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(50);

                    b.Property<string>("CreatedByProvider")
                        .HasMaxLength(50);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("EncryptedText")
                        .IsRequired();

                    b.Property<string>("EncryptionType")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedByProvider");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Secret");
                });

            modelBuilder.Entity("Passaword.SecretProperty", b =>
                {
                    b.Property<string>("SecretId")
                        .HasMaxLength(32);

                    b.Property<string>("Type")
                        .HasMaxLength(250);

                    b.Property<string>("Data");

                    b.HasKey("SecretId", "Type")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("SecretProperty");
                });

            modelBuilder.Entity("Passaword.Validation.SecretValidationRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SecretId")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("ValidationData");

                    b.Property<string>("Validator")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("SecretId");

                    b.ToTable("SecretValidationRule");
                });

            modelBuilder.Entity("Passaword.SecretProperty", b =>
                {
                    b.HasOne("Passaword.Secret", "Secret")
                        .WithMany("SecretProperties")
                        .HasForeignKey("SecretId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Passaword.Validation.SecretValidationRule", b =>
                {
                    b.HasOne("Passaword.Secret", "Secret")
                        .WithMany("SecretValidationRules")
                        .HasForeignKey("SecretId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

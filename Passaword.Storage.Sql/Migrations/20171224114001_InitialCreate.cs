using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Passaword.Storage.Sql.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Secret",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 32, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedByProvider = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    EncryptedText = table.Column<string>(nullable: false),
                    EncryptionType = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secret", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecretProperty",
                columns: table => new
                {
                    SecretId = table.Column<string>(maxLength: 32, nullable: false),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretProperty", x => new { x.SecretId, x.Type })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_SecretProperty_Secret_SecretId",
                        column: x => x.SecretId,
                        principalTable: "Secret",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecretValidationRule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SecretId = table.Column<string>(maxLength: 32, nullable: false),
                    ValidationData = table.Column<string>(nullable: true),
                    Validator = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretValidationRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecretValidationRule_Secret_SecretId",
                        column: x => x.SecretId,
                        principalTable: "Secret",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Secret_CreatedBy",
                table: "Secret",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Secret_CreatedByProvider",
                table: "Secret",
                column: "CreatedByProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Secret_Id",
                table: "Secret",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecretValidationRule_SecretId",
                table: "SecretValidationRule",
                column: "SecretId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecretProperty");

            migrationBuilder.DropTable(
                name: "SecretValidationRule");

            migrationBuilder.DropTable(
                name: "Secret");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheFullStackTeam.Infrastructure.Persistence.Sql.Migrations;
/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Accounts",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Roles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                PasswordResetToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                PasswordResetExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Accounts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RefreshTokens",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                JwtId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsUsed = table.Column<bool>(type: "bit", nullable: false),
                IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                table.ForeignKey(
                    name: "FK_RefreshTokens_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserProfiles",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserProfiles", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserProfiles_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Accounts_Email",
            table: "Accounts",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_RefreshTokens_AccountId",
            table: "RefreshTokens",
            column: "AccountId");

        migrationBuilder.CreateIndex(
            name: "IX_UserProfiles_AccountId",
            table: "UserProfiles",
            column: "AccountId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "RefreshTokens");

        migrationBuilder.DropTable(
            name: "UserProfiles");

        migrationBuilder.DropTable(
            name: "Accounts");
    }
}

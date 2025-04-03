using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddingColumnToUserMagTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenExpiry",
                table: "UserMagTables",
                newName: "PasswordTokenExpiry");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailTokenExpiry",
                table: "UserMagTables",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "UserMagTables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccountLocked",
                table: "UserMagTables",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "UserMagTables",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PendingEmail",
                table: "UserMagTables",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailTokenExpiry",
                table: "UserMagTables");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "UserMagTables");

            migrationBuilder.DropColumn(
                name: "IsAccountLocked",
                table: "UserMagTables");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "UserMagTables");

            migrationBuilder.DropColumn(
                name: "PendingEmail",
                table: "UserMagTables");

            migrationBuilder.RenameColumn(
                name: "PasswordTokenExpiry",
                table: "UserMagTables",
                newName: "TokenExpiry");
        }
    }
}

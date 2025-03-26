using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddingColumnToUserMagTableandTaskMagTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationToken",
                table: "UserMagTables",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailConfirmed",
                table: "UserMagTables",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "UserMagTables",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "UserMagTables",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiry",
                table: "UserMagTables",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "TaskMagTables",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationToken",
                table: "UserMagTables");

            migrationBuilder.DropColumn(
                name: "IsEmailConfirmed",
                table: "UserMagTables");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "UserMagTables");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "UserMagTables");

            migrationBuilder.DropColumn(
                name: "TokenExpiry",
                table: "UserMagTables");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "TaskMagTables");
        }
    }
}

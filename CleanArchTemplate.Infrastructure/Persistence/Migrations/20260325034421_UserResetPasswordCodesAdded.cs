using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchTemplate.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserResetPasswordCodesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "ResetPasswordCodes",
                table: "Users",
                type: "text[]",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE "Users"
                SET "ResetPasswordCodes" = ARRAY[]::text[]
                WHERE "ResetPasswordCodes" IS NULL;
                """);

            migrationBuilder.AlterColumn<List<string>>(
                name: "ResetPasswordCodes",
                table: "Users",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordCodes",
                table: "Users");
        }
    }
}

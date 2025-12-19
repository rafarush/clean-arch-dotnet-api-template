using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchTemplate.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RoleCorrectedV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policy_Users_UserId",
                table: "Policy");

            migrationBuilder.DropIndex(
                name: "IX_Policy_UserId",
                table: "Policy");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Policy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Policy",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policy_UserId",
                table: "Policy",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Policy_Users_UserId",
                table: "Policy",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

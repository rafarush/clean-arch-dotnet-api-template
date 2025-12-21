using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchTemplate.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RoleCorrections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PolicyRole_Policy_PoliciesId",
                table: "PolicyRole");

            migrationBuilder.DropForeignKey(
                name: "FK_PolicyRole_Role_RolesId",
                table: "PolicyRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Role_RolesId",
                table: "RoleUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Policy",
                table: "Policy");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "Policy",
                newName: "Policies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Policies",
                table: "Policies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyRole_Policies_PoliciesId",
                table: "PolicyRole",
                column: "PoliciesId",
                principalTable: "Policies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyRole_Roles_RolesId",
                table: "PolicyRole",
                column: "RolesId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Roles_RolesId",
                table: "RoleUser",
                column: "RolesId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PolicyRole_Policies_PoliciesId",
                table: "PolicyRole");

            migrationBuilder.DropForeignKey(
                name: "FK_PolicyRole_Roles_RolesId",
                table: "PolicyRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Roles_RolesId",
                table: "RoleUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Policies",
                table: "Policies");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Role");

            migrationBuilder.RenameTable(
                name: "Policies",
                newName: "Policy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Policy",
                table: "Policy",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyRole_Policy_PoliciesId",
                table: "PolicyRole",
                column: "PoliciesId",
                principalTable: "Policy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyRole_Role_RolesId",
                table: "PolicyRole",
                column: "RolesId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Role_RolesId",
                table: "RoleUser",
                column: "RolesId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

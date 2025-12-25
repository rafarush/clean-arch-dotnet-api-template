using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchTemplate.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserPassToByteArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Reemplaza el AlterColumn automático con SQL personalizado
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ALTER COLUMN ""Password"" TYPE bytea 
                USING ""Password""::bytea;
            ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ALTER COLUMN ""Password"" TYPE text 
                USING ""Password""::text;
            ");
        }

    }
}

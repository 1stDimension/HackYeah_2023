using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HackYeah.Backend.Keystore.Data.Migrations
{
    /// <inheritdoc />
    public partial class UniqueName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_key_name",
                table: "keys");

            migrationBuilder.AddUniqueConstraint(
                name: "ukey_key_name",
                table: "keys",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_key_name",
                table: "keys",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "ukey_key_name",
                table: "keys");

            migrationBuilder.DropIndex(
                name: "ix_key_name",
                table: "keys");

            migrationBuilder.CreateIndex(
                name: "ix_key_name",
                table: "keys",
                column: "name");
        }
    }
}

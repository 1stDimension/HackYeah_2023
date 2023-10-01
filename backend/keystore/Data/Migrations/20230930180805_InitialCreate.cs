using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HackYeah.Backend.Keystore.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "keys",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    size = table.Column<uint>(type: "INTEGER", nullable: false),
                    priv = table.Column<byte[]>(type: "BLOB", nullable: false),
                    pub = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pkey_key_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_key_name",
                table: "keys",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_key_size",
                table: "keys",
                column: "size");

            migrationBuilder.CreateIndex(
                name: "ix_key_type",
                table: "keys",
                column: "type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "keys");
        }
    }
}

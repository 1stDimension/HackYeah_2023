using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HackYeah.Backend.Keystore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "priv",
                table: "keys",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.CreateTable(
                name: "certificates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    cn = table.Column<string>(type: "TEXT", nullable: false),
                    thumbprint = table.Column<string>(type: "TEXT", nullable: false),
                    data = table.Column<byte[]>(type: "BLOB", nullable: false),
                    keypair = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pkey_certiifcates_id", x => x.id);
                    table.UniqueConstraint("ukey_certificates_thumbprint", x => x.thumbprint);
                    table.ForeignKey(
                        name: "fkey_certificates_keypair",
                        column: x => x.keypair,
                        principalTable: "keys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_certificates_cn",
                table: "certificates",
                column: "cn");

            migrationBuilder.CreateIndex(
                name: "ix_certificates_keypair",
                table: "certificates",
                column: "keypair");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "certificates");

            migrationBuilder.AlterColumn<byte[]>(
                name: "priv",
                table: "keys",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);
        }
    }
}

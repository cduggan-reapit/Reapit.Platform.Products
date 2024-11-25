using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reapit.Platform.Products.Data.Context.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dummies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    date_created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_modified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dummies", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dummies_date_created",
                table: "dummies",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_dummies_date_modified",
                table: "dummies",
                column: "date_modified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dummies");
        }
    }
}

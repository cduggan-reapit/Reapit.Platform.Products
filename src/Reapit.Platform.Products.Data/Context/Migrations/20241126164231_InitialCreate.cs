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
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_deleted = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "product_clients",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    productId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_deleted = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    clientId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    grantId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(140)", maxLength: 140, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<int>(type: "int", nullable: false),
                    callbackUrls = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signOutUrls = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_clients", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_clients_products_productId",
                        column: x => x.productId,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_product_clients_cursor",
                table: "product_clients",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_clients_date_created",
                table: "product_clients",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_product_clients_date_deleted",
                table: "product_clients",
                column: "date_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_product_clients_date_modified",
                table: "product_clients",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_product_clients_name_date_deleted",
                table: "product_clients",
                columns: new[] { "name", "date_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_clients_productId",
                table: "product_clients",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_products_cursor",
                table: "products",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_date_created",
                table: "products",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_products_date_deleted",
                table: "products",
                column: "date_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_products_date_modified",
                table: "products",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_products_name",
                table: "products",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_clients");

            migrationBuilder.DropTable(
                name: "products");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
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
                name: "applications",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_first_party = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_deleted = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "resource_servers",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    externalId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    audience = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tokenLifetime = table.Column<int>(type: "int", nullable: false),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_deleted = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_servers", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    appId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    externalId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(140)", maxLength: 140, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    loginUrl = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    callbackUrls = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signOutUrls = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_deleted = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.id);
                    table.ForeignKey(
                        name: "FK_clients_applications_appId",
                        column: x => x.appId,
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "scopes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    value = table.Column<string>(type: "varchar(280)", maxLength: 280, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    resourceServerId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_scopes_resource_servers_resourceServerId",
                        column: x => x.resourceServerId,
                        principalTable: "resource_servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "grants",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    externalId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    clientId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    resourceServerId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_deleted = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grants", x => x.id);
                    table.ForeignKey(
                        name: "FK_grants_clients_clientId",
                        column: x => x.clientId,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_grants_resource_servers_resourceServerId",
                        column: x => x.resourceServerId,
                        principalTable: "resource_servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "grant_scopes",
                columns: table => new
                {
                    grantId = table.Column<string>(type: "varchar(36)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    scopeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grant_scopes", x => new { x.grantId, x.scopeId });
                    table.ForeignKey(
                        name: "FK_grant_scopes_grants_grantId",
                        column: x => x.grantId,
                        principalTable: "grants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_grant_scopes_scopes_scopeId",
                        column: x => x.scopeId,
                        principalTable: "scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_applications_cursor",
                table: "applications",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_applications_date_created",
                table: "applications",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_applications_date_deleted",
                table: "applications",
                column: "date_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_applications_date_modified",
                table: "applications",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_applications_name_date_deleted",
                table: "applications",
                columns: new[] { "name", "date_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clients_appId",
                table: "clients",
                column: "appId");

            migrationBuilder.CreateIndex(
                name: "IX_clients_cursor",
                table: "clients",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clients_date_created",
                table: "clients",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_clients_date_deleted",
                table: "clients",
                column: "date_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_clients_date_modified",
                table: "clients",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_clients_name_date_deleted",
                table: "clients",
                columns: new[] { "name", "date_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_grant_scopes_scopeId",
                table: "grant_scopes",
                column: "scopeId");

            migrationBuilder.CreateIndex(
                name: "IX_grants_clientId_resourceServerId_date_deleted",
                table: "grants",
                columns: new[] { "clientId", "resourceServerId", "date_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_grants_cursor",
                table: "grants",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_grants_date_created",
                table: "grants",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_grants_date_deleted",
                table: "grants",
                column: "date_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_grants_date_modified",
                table: "grants",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_grants_resourceServerId",
                table: "grants",
                column: "resourceServerId");

            migrationBuilder.CreateIndex(
                name: "IX_resource_servers_cursor",
                table: "resource_servers",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resource_servers_date_created",
                table: "resource_servers",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_resource_servers_date_deleted",
                table: "resource_servers",
                column: "date_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_resource_servers_date_modified",
                table: "resource_servers",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_resource_servers_name_date_deleted",
                table: "resource_servers",
                columns: new[] { "name", "date_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scopes_resourceServerId",
                table: "scopes",
                column: "resourceServerId");

            migrationBuilder.CreateIndex(
                name: "IX_scopes_value_resourceServerId",
                table: "scopes",
                columns: new[] { "value", "resourceServerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grant_scopes");

            migrationBuilder.DropTable(
                name: "grants");

            migrationBuilder.DropTable(
                name: "scopes");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "resource_servers");

            migrationBuilder.DropTable(
                name: "applications");
        }
    }
}

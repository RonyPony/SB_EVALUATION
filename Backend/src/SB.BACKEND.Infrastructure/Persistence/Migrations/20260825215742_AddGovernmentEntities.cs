using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SB.BACKEND.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGovernmentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntidadesGubernamentales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NombreNormalizado = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PoderDelEstado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sector = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntidadesGubernamentales", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000012"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Allows GOVERNMENT_ENTITY.VIEW.", true, "GOVERNMENT_ENTITY.VIEW", "GOVERNMENT_ENTITY.VIEW", null },
                    { new Guid("10000000-0000-0000-0000-000000000013"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Allows GOVERNMENT_ENTITY.CREATE.", true, "GOVERNMENT_ENTITY.CREATE", "GOVERNMENT_ENTITY.CREATE", null },
                    { new Guid("10000000-0000-0000-0000-000000000014"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Allows GOVERNMENT_ENTITY.UPDATE.", true, "GOVERNMENT_ENTITY.UPDATE", "GOVERNMENT_ENTITY.UPDATE", null },
                    { new Guid("10000000-0000-0000-0000-000000000015"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Allows GOVERNMENT_ENTITY.DELETE.", true, "GOVERNMENT_ENTITY.DELETE", "GOVERNMENT_ENTITY.DELETE", null },
                    { new Guid("10000000-0000-0000-0000-000000000016"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Allows GOVERNMENT_ENTITY.RESTORE.", true, "GOVERNMENT_ENTITY.RESTORE", "GOVERNMENT_ENTITY.RESTORE", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntidadesGubernamentales_Categoria",
                table: "EntidadesGubernamentales",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_EntidadesGubernamentales_IsDeleted_Activo",
                table: "EntidadesGubernamentales",
                columns: new[] { "IsDeleted", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_EntidadesGubernamentales_NombreNormalizado",
                table: "EntidadesGubernamentales",
                column: "NombreNormalizado",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EntidadesGubernamentales_PoderDelEstado",
                table: "EntidadesGubernamentales",
                column: "PoderDelEstado");

            migrationBuilder.CreateIndex(
                name: "IX_EntidadesGubernamentales_Sector",
                table: "EntidadesGubernamentales",
                column: "Sector");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntidadesGubernamentales");

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000016"));
        }
    }
}

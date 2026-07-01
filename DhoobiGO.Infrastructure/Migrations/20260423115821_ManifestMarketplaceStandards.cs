using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DhoobiGO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ManifestMarketplaceStandards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "ServiceTypes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ServiceTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "Id", "BasePrice", "Category", "CreatedAt", "Description", "Icon", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 101, 50m, "Cleaning", new DateTime(2026, 4, 23, 11, 58, 20, 839, DateTimeKind.Utc).AddTicks(5537), "Professional machine wash and tumble dry", "water", "Washing", null },
                    { 102, 20m, "Ironing", new DateTime(2026, 4, 23, 11, 58, 20, 839, DateTimeKind.Utc).AddTicks(7269), "Steam iron and hangers/folding", "flash", "Ironing", null },
                    { 103, 300m, "Special", new DateTime(2026, 4, 23, 11, 58, 20, 839, DateTimeKind.Utc).AddTicks(7273), "Delicate chemical cleaning for premium wear", "star", "Dry Clean", null },
                    { 104, 500m, "Cleaning", new DateTime(2026, 4, 23, 11, 58, 20, 839, DateTimeKind.Utc).AddTicks(7275), "Blankets, curtains and heavy fabrics", "bed", "Heavy Duty", null },
                    { 105, 100m, "Ironing", new DateTime(2026, 4, 23, 11, 58, 20, 839, DateTimeKind.Utc).AddTicks(7276), "Premium steam pressing for suits and dresses", "shirt", "Steam Press", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "ServiceTypes");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ServiceTypes");
        }
    }
}

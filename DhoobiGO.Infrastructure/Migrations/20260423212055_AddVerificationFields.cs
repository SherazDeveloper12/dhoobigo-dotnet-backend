using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DhoobiGO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CnicNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShopName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationImageUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 21, 20, 53, 794, DateTimeKind.Utc).AddTicks(8264));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 21, 20, 53, 794, DateTimeKind.Utc).AddTicks(9411));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 21, 20, 53, 794, DateTimeKind.Utc).AddTicks(9414));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 21, 20, 53, 794, DateTimeKind.Utc).AddTicks(9415));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 21, 20, 53, 794, DateTimeKind.Utc).AddTicks(9417));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CnicNumber", "ShopName", "VerificationImageUrl" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CnicNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShopName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerificationImageUrl",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 12, 11, 14, 349, DateTimeKind.Utc).AddTicks(6417));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 12, 11, 14, 349, DateTimeKind.Utc).AddTicks(9265));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 12, 11, 14, 349, DateTimeKind.Utc).AddTicks(9273));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 12, 11, 14, 349, DateTimeKind.Utc).AddTicks(9276));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 12, 11, 14, 349, DateTimeKind.Utc).AddTicks(9278));
        }
    }
}

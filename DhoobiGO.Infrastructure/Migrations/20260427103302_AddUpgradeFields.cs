using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DhoobiGO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUpgradeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUpgradePending",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RequestedDhobiType",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpgradeDocsUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InsuranceFee",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsInsured",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 27, 10, 32, 58, 54, DateTimeKind.Utc).AddTicks(8977));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 27, 10, 32, 58, 55, DateTimeKind.Utc).AddTicks(4876));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 27, 10, 32, 58, 55, DateTimeKind.Utc).AddTicks(4891));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 27, 10, 32, 58, 55, DateTimeKind.Utc).AddTicks(4897));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 27, 10, 32, 58, 55, DateTimeKind.Utc).AddTicks(4903));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsUpgradePending", "RequestedDhobiType", "UpgradeDocsUrl" },
                values: new object[] { false, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUpgradePending",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RequestedDhobiType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpgradeDocsUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InsuranceFee",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsInsured",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 27, 10, 5, 10, 152, DateTimeKind.Utc).AddTicks(6763));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 27, 10, 5, 10, 152, DateTimeKind.Utc).AddTicks(9967));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 27, 10, 5, 10, 152, DateTimeKind.Utc).AddTicks(9974));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 27, 10, 5, 10, 152, DateTimeKind.Utc).AddTicks(9977));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 27, 10, 5, 10, 152, DateTimeKind.Utc).AddTicks(9979));
        }
    }
}

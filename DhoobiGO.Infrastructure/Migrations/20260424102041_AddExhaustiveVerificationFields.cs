using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DhoobiGO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExhaustiveVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VerificationImageUrl",
                table: "Users",
                newName: "VehicleRegistrationUrl");

            migrationBuilder.AddColumn<string>(
                name: "BusinessLicenseUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CnicImageUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrivingLicenseUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ElectricityBillUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentImageUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoliceVerificationUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelfieWithIdUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleImageUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 24, 10, 20, 38, 963, DateTimeKind.Utc).AddTicks(1269));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 24, 10, 20, 38, 963, DateTimeKind.Utc).AddTicks(3172));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 24, 10, 20, 38, 963, DateTimeKind.Utc).AddTicks(3177));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 24, 10, 20, 38, 963, DateTimeKind.Utc).AddTicks(3178));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 24, 10, 20, 38, 963, DateTimeKind.Utc).AddTicks(3180));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BusinessLicenseUrl", "CnicImageUrl", "DrivingLicenseUrl", "ElectricityBillUrl", "EquipmentImageUrl", "PoliceVerificationUrl", "SelfieWithIdUrl", "VehicleImageUrl", "VehicleNumber" },
                values: new object[] { null, null, null, null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessLicenseUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CnicImageUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ElectricityBillUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EquipmentImageUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PoliceVerificationUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelfieWithIdUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VehicleImageUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VehicleNumber",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "VehicleRegistrationUrl",
                table: "Users",
                newName: "VerificationImageUrl");

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
        }
    }
}

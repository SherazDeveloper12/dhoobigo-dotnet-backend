using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DhoobiGO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRiderToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RiderId",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 26, 20, 31, 17, 684, DateTimeKind.Utc).AddTicks(4184));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 26, 20, 31, 17, 684, DateTimeKind.Utc).AddTicks(5543));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 26, 20, 31, 17, 684, DateTimeKind.Utc).AddTicks(5546));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 26, 20, 31, 17, 684, DateTimeKind.Utc).AddTicks(5547));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 26, 20, 31, 17, 684, DateTimeKind.Utc).AddTicks(5548));

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RiderId",
                table: "Reviews",
                column: "RiderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_RiderId",
                table: "Reviews",
                column: "RiderId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_RiderId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_RiderId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "RiderId",
                table: "Reviews");

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 26, 20, 23, 50, 605, DateTimeKind.Utc).AddTicks(4713));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 26, 20, 23, 50, 605, DateTimeKind.Utc).AddTicks(6272));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 26, 20, 23, 50, 605, DateTimeKind.Utc).AddTicks(6275));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 26, 20, 23, 50, 605, DateTimeKind.Utc).AddTicks(6277));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 26, 20, 23, 50, 605, DateTimeKind.Utc).AddTicks(6278));
        }
    }
}

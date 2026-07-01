using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DhoobiGO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRiderNegotiationLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RiderFee",
                table: "Orders",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RiderBids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    RiderId = table.Column<int>(type: "integer", nullable: false),
                    OfferedFee = table.Column<decimal>(type: "numeric", nullable: false),
                    OfferedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiderBids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiderBids_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RiderBids_Users_RiderId",
                        column: x => x.RiderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_RiderBids_OrderId",
                table: "RiderBids",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RiderBids_RiderId",
                table: "RiderBids",
                column: "RiderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RiderBids");

            migrationBuilder.DropColumn(
                name: "RiderFee",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 11, 58, 20, 839, DateTimeKind.Utc).AddTicks(5537));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 11, 58, 20, 839, DateTimeKind.Utc).AddTicks(7269));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 11, 58, 20, 839, DateTimeKind.Utc).AddTicks(7273));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 11, 58, 20, 839, DateTimeKind.Utc).AddTicks(7275));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 23, 11, 58, 20, 839, DateTimeKind.Utc).AddTicks(7276));
        }
    }
}

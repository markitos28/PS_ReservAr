using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservAr.Migrations
{
    /// <inheritdoc />
    public partial class FixEventSeedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Event",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EventDate", "Status" },
                values: new object[] { new DateTime(2026, 6, 16, 21, 0, 0, 0, DateTimeKind.Utc), "DISPONIBLE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Event",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EventDate", "Status" },
                values: new object[] { new DateTime(2026, 6, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Activo" });
        }
    }
}

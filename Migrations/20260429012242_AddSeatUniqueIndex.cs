using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservAr.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seat_SectorId",
                table: "Seat");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_SectorId_RowIdentifier_SeatNumber",
                table: "Seat",
                columns: new[] { "SectorId", "RowIdentifier", "SeatNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seat_SectorId_RowIdentifier_SeatNumber",
                table: "Seat");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_SectorId",
                table: "Seat",
                column: "SectorId");
        }
    }
}

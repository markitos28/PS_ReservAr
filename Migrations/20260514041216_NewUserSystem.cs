using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservAr.Migrations
{
    /// <inheritdoc />
    public partial class NewUserSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Name", "PasswordHash" },
                values: new object[] { -1, "system_application@reservar.com", "System Application", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -1);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReservAr.Migrations
{
    /// <inheritdoc />
    public partial class DataSeeding_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Seat",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reservation",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Audit_Log",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.InsertData(
                table: "Event",
                columns: new[] { "Id", "EventDate", "Name", "Status", "Venue" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Noche de Rock Amateur", "Activo", "Niceto Club - CABA" });

            migrationBuilder.InsertData(
                table: "Sector",
                columns: new[] { "Id", "Capacity", "EventId", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 50, 1, "Platea Alta", 25000.00m },
                    { 2, 50, 1, "Platea Baja", 10000.00m }
                });

            migrationBuilder.InsertData(
                table: "Seat",
                columns: new[] { "Id", "RowIdentifier", "SeatNumber", "SectorId", "Status" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "A", 1, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "A", 2, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "A", 3, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "A", 4, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "A", 5, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "A", 6, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "A", 7, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "A", 8, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "A", 9, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "A", 10, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "B", 11, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "B", 12, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "B", 13, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "B", 14, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "B", 15, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "B", 16, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "B", 17, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "B", 18, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "B", 19, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "B", 20, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "C", 21, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "C", 22, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000023"), "C", 23, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000024"), "C", 24, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000025"), "C", 25, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000026"), "C", 26, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000027"), "C", 27, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000028"), "C", 28, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000029"), "C", 29, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000030"), "C", 30, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000031"), "D", 31, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000032"), "D", 32, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000033"), "D", 33, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000034"), "D", 34, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000035"), "D", 35, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000036"), "D", 36, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000037"), "D", 37, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000038"), "D", 38, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000039"), "D", 39, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000040"), "D", 40, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000041"), "E", 41, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000042"), "E", 42, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000043"), "E", 43, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000044"), "E", 44, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000045"), "E", 45, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000046"), "E", 46, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000047"), "E", 47, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000048"), "E", 48, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000049"), "E", 49, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000050"), "E", 50, 1, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000051"), "A", 1, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000052"), "A", 2, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000053"), "A", 3, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000054"), "A", 4, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000055"), "A", 5, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000056"), "A", 6, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000057"), "A", 7, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000058"), "A", 8, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000059"), "A", 9, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000060"), "A", 10, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000061"), "B", 11, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000062"), "B", 12, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000063"), "B", 13, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000064"), "B", 14, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000065"), "B", 15, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000066"), "B", 16, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000067"), "B", 17, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000068"), "B", 18, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000069"), "B", 19, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000070"), "B", 20, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000071"), "C", 21, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000072"), "C", 22, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000073"), "C", 23, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000074"), "C", 24, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000075"), "C", 25, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000076"), "C", 26, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000077"), "C", 27, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000078"), "C", 28, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000079"), "C", 29, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000080"), "C", 30, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000081"), "D", 31, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000082"), "D", 32, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000083"), "D", 33, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000084"), "D", 34, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000085"), "D", 35, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000086"), "D", 36, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000087"), "D", 37, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000088"), "D", 38, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000089"), "D", 39, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000090"), "D", 40, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000091"), "E", 41, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000092"), "E", 42, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000093"), "E", 43, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000094"), "E", 44, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000095"), "E", 45, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000096"), "E", 46, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000097"), "E", 47, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000098"), "E", 48, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000099"), "E", 49, 2, "Disponible" },
                    { new Guid("00000000-0000-0000-0000-000000000100"), "E", 50, 2, "Disponible" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000026"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000027"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000028"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000029"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000030"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000031"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000032"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000033"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000034"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000035"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000036"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000037"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000038"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000039"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000040"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000041"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000042"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000043"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000044"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000045"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000046"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000047"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000048"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000049"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000050"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000051"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000052"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000053"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000054"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000055"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000056"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000057"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000058"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000059"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000060"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000061"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000062"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000063"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000064"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000065"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000066"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000067"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000068"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000069"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000070"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000071"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000072"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000073"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000074"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000075"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000076"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000077"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000078"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000079"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000080"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000081"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000082"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000083"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000084"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000085"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000086"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000087"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000088"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000089"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000090"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000091"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000092"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000093"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000094"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000095"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000096"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000097"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000098"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000099"));

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000100"));

            migrationBuilder.DeleteData(
                table: "Sector",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sector",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Event",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Seat",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reservation",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Audit_Log",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}

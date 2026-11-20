using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestionEventiMagazzino.Migrations
{
    /// <inheritdoc />
    public partial class SeedStatiIniziali : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "StatiEventi",
                columns: new[] { "statEvent", "descrEvento", "note" },
                values: new object[,]
                {
                    { "CANCELLATO", "Evento cancellato", null },
                    { "CHIUSO", "Evento concluso", null },
                    { "IN CORSO", "Evento in corso", null },
                    { "SOSPESO", "Evento sospeso", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StatiEventi",
                keyColumn: "statEvent",
                keyValue: "CANCELLATO");

            migrationBuilder.DeleteData(
                table: "StatiEventi",
                keyColumn: "statEvent",
                keyValue: "CHIUSO");

            migrationBuilder.DeleteData(
                table: "StatiEventi",
                keyColumn: "statEvent",
                keyValue: "IN CORSO");

            migrationBuilder.DeleteData(
                table: "StatiEventi",
                keyColumn: "statEvent",
                keyValue: "SOSPESO");
        }
    }
}

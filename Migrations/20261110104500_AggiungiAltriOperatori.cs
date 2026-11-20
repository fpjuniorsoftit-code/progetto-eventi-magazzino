using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionEventiMagazzino.Migrations
{
    /// <inheritdoc />
    public partial class AggiungiAltriOperatori : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isAdmin",
                table: "Users",
                newName: "IsAdmin");

            migrationBuilder.AddColumn<string>(
                name: "altriOperatori",
                table: "Events",
                type: "nvarchar(255)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "altriOperatori",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "IsAdmin",
                table: "Users",
                newName: "isAdmin");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionEventiMagazzino.Migrations
{
    /// <inheritdoc />
    public partial class aggiuntaIsDeactivated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDeactivated",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeactivated",
                table: "Users");
        }
    }
}

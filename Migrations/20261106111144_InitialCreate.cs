using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionEventiMagazzino.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatiEventi",
                columns: table => new
                {
                    statEvent = table.Column<string>(type: "varchar(20)", nullable: false),
                    descrEvento = table.Column<string>(type: "varchar(50)", nullable: false),
                    note = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatiEventi", x => x.statEvent);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    passwordHash = table.Column<string>(type: "varchar(500)", nullable: false),
                    email = table.Column<string>(type: "varchar(150)", nullable: false),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.username);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    eventoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    timeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dataEvento = table.Column<DateOnly>(type: "date", nullable: false),
                    oraInizio = table.Column<TimeOnly>(type: "time", nullable: false),
                    oraFine = table.Column<TimeOnly>(type: "time", nullable: false),
                    evento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    stato = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.eventoID);
                    table.ForeignKey(
                        name: "FK_Events_StatiEventi_stato",
                        column: x => x.stato,
                        principalTable: "StatiEventi",
                        principalColumn: "statEvent",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Users_username",
                        column: x => x.username,
                        principalTable: "Users",
                        principalColumn: "username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_stato",
                table: "Events",
                column: "stato");

            migrationBuilder.CreateIndex(
                name: "IX_Events_username",
                table: "Events",
                column: "username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "StatiEventi");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

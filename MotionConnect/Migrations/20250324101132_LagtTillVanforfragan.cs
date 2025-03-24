using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotionConnect.Migrations
{
    /// <inheritdoc />
    public partial class LagtTillVanforfragan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vanforfrågningar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvsandareId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MottagareId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Skickades = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArGodkand = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vanforfrågningar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vanforfrågningar_AspNetUsers_AvsandareId",
                        column: x => x.AvsandareId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vanforfrågningar_AspNetUsers_MottagareId",
                        column: x => x.MottagareId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vanforfrågningar_AvsandareId",
                table: "Vanforfrågningar",
                column: "AvsandareId");

            migrationBuilder.CreateIndex(
                name: "IX_Vanforfrågningar_MottagareId",
                table: "Vanforfrågningar",
                column: "MottagareId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vanforfrågningar");
        }
    }
}

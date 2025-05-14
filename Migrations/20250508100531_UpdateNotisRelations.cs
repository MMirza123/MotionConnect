using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotionConnect.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotisRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notiser_AspNetUsers_AnvandarId",
                table: "Notiser");

            migrationBuilder.AddColumn<string>(
                name: "AvsandareId",
                table: "Notiser",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Notiser_AvsandareId",
                table: "Notiser",
                column: "AvsandareId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notiser_AspNetUsers_AnvandarId",
                table: "Notiser",
                column: "AnvandarId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notiser_AspNetUsers_AvsandareId",
                table: "Notiser",
                column: "AvsandareId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notiser_AspNetUsers_AnvandarId",
                table: "Notiser");

            migrationBuilder.DropForeignKey(
                name: "FK_Notiser_AspNetUsers_AvsandareId",
                table: "Notiser");

            migrationBuilder.DropIndex(
                name: "IX_Notiser_AvsandareId",
                table: "Notiser");

            migrationBuilder.DropColumn(
                name: "AvsandareId",
                table: "Notiser");

            migrationBuilder.AddForeignKey(
                name: "FK_Notiser_AspNetUsers_AnvandarId",
                table: "Notiser",
                column: "AnvandarId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

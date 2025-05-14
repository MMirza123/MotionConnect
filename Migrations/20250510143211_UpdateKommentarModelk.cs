using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotionConnect.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKommentarModelk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kommentarer_AspNetUsers_AnvandareId",
                table: "Kommentarer");

            migrationBuilder.DropColumn(
                name: "AnvandarId",
                table: "Kommentarer");

            migrationBuilder.AlterColumn<string>(
                name: "AnvandareId",
                table: "Kommentarer",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Kommentarer_AspNetUsers_AnvandareId",
                table: "Kommentarer",
                column: "AnvandareId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kommentarer_AspNetUsers_AnvandareId",
                table: "Kommentarer");

            migrationBuilder.AlterColumn<string>(
                name: "AnvandareId",
                table: "Kommentarer",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "AnvandarId",
                table: "Kommentarer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Kommentarer_AspNetUsers_AnvandareId",
                table: "Kommentarer",
                column: "AnvandareId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

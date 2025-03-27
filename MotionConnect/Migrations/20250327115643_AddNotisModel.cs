using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotionConnect.Migrations
{
    /// <inheritdoc />
    public partial class AddNotisModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Meddealande",
                table: "Notiser",
                newName: "Meddelande");

            migrationBuilder.AddColumn<int>(
                name: "InlaggId",
                table: "Notiser",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MeddelandeId",
                table: "Notiser",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Typ",
                table: "Notiser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notiser_InlaggId",
                table: "Notiser",
                column: "InlaggId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notiser_Inlagg_InlaggId",
                table: "Notiser",
                column: "InlaggId",
                principalTable: "Inlagg",
                principalColumn: "InlaggId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notiser_Inlagg_InlaggId",
                table: "Notiser");

            migrationBuilder.DropIndex(
                name: "IX_Notiser_InlaggId",
                table: "Notiser");

            migrationBuilder.DropColumn(
                name: "InlaggId",
                table: "Notiser");

            migrationBuilder.DropColumn(
                name: "MeddelandeId",
                table: "Notiser");

            migrationBuilder.DropColumn(
                name: "Typ",
                table: "Notiser");

            migrationBuilder.RenameColumn(
                name: "Meddelande",
                table: "Notiser",
                newName: "Meddealande");
        }
    }
}

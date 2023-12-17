using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDP_Project_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TierId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TierId",
                table: "Users",
                column: "TierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tiers_TierId",
                table: "Users",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tiers_TierId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TierId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TierId",
                table: "Users");
        }
    }
}

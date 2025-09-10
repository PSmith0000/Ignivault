using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ignivault.API.Migrations
{
    /// <inheritdoc />
    public partial class Rebuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaultItems_AspNetUsers_UserId",
                table: "VaultItems");

            migrationBuilder.DropIndex(
                name: "IX_VaultItems_UserId",
                table: "VaultItems");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "VaultItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "LoginUserId",
                table: "VaultItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_LoginUserId",
                table: "VaultItems",
                column: "LoginUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VaultItems_AspNetUsers_LoginUserId",
                table: "VaultItems",
                column: "LoginUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaultItems_AspNetUsers_LoginUserId",
                table: "VaultItems");

            migrationBuilder.DropIndex(
                name: "IX_VaultItems_LoginUserId",
                table: "VaultItems");

            migrationBuilder.DropColumn(
                name: "LoginUserId",
                table: "VaultItems");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "VaultItems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_UserId",
                table: "VaultItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VaultItems_AspNetUsers_UserId",
                table: "VaultItems",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ignivault.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIVToLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "MasterIV",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MasterIV",
                table: "AspNetUsers");
        }
    }
}

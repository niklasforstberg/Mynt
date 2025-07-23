using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mynt.Migrations
{
    /// <inheritdoc />
    public partial class ModifyUser_Add_settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Settings",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Settings",
                table: "Users");
        }
    }
}

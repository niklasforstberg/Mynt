using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mynt.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableCurrencyAddSymbolPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SymbolPosition",
                table: "Currencies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SymbolPosition",
                table: "Currencies");
        }
    }
}

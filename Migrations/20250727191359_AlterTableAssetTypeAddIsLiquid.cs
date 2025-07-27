using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mynt.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableAssetTypeAddIsLiquid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLiquid",
                table: "AssetTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLiquid",
                table: "AssetTypes");
        }
    }
}

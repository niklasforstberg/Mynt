using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mynt.Migrations
{
    /// <inheritdoc />
    public partial class AlterCurrencyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Currencies_CurrencyId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_FromCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_ToCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyExchangeRates_FromCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyExchangeRates_ToCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Assets_CurrencyId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "FromCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropColumn(
                name: "ToCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Assets");

            migrationBuilder.AddColumn<string>(
                name: "FromCurrencyCode",
                table: "CurrencyExchangeRates",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToCurrencyCode",
                table: "CurrencyExchangeRates",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Assets",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_FromCurrencyCode",
                table: "CurrencyExchangeRates",
                column: "FromCurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_ToCurrencyCode",
                table: "CurrencyExchangeRates",
                column: "ToCurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CurrencyCode",
                table: "Assets",
                column: "CurrencyCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Currencies_CurrencyCode",
                table: "Assets",
                column: "CurrencyCode",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_FromCurrencyCode",
                table: "CurrencyExchangeRates",
                column: "FromCurrencyCode",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_ToCurrencyCode",
                table: "CurrencyExchangeRates",
                column: "ToCurrencyCode",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Currencies_CurrencyCode",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_FromCurrencyCode",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_ToCurrencyCode",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyExchangeRates_FromCurrencyCode",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyExchangeRates_ToCurrencyCode",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Assets_CurrencyCode",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "FromCurrencyCode",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropColumn(
                name: "ToCurrencyCode",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Assets");

            migrationBuilder.AddColumn<int>(
                name: "FromCurrencyId",
                table: "CurrencyExchangeRates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ToCurrencyId",
                table: "CurrencyExchangeRates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Currencies",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Assets",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_FromCurrencyId",
                table: "CurrencyExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_ToCurrencyId",
                table: "CurrencyExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CurrencyId",
                table: "Assets",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Currencies_CurrencyId",
                table: "Assets",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_FromCurrencyId",
                table: "CurrencyExchangeRates",
                column: "FromCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_ToCurrencyId",
                table: "CurrencyExchangeRates",
                column: "ToCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

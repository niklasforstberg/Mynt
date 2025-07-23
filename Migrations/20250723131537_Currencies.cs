using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mynt.Migrations
{
    /// <inheritdoc />
    public partial class Currencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Assets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsSystemManaged = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Currencies_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromCurrencyId = table.Column<int>(type: "int", nullable: false),
                    ToCurrencyId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SetById = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRates_Currencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRates_Currencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRates_Users_SetById",
                        column: x => x.SetById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CurrencyId",
                table: "Assets",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CreatedById",
                table: "Currencies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_FromCurrencyId",
                table: "CurrencyExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_SetById",
                table: "CurrencyExchangeRates",
                column: "SetById");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_ToCurrencyId",
                table: "CurrencyExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Currencies_CurrencyId",
                table: "Assets",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Currencies_CurrencyId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "CurrencyExchangeRates");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Assets_CurrencyId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Assets");
        }
    }
}

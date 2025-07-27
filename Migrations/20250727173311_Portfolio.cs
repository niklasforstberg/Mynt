using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mynt.Migrations
{
    /// <inheritdoc />
    public partial class Portfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortfolioSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FinancialGroupId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssetSummary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DebtSummary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalSummary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GroupAssetSummary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GroupDebtSummary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GroupTotalSummary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PercentageChange = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GroupPercentageChange = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioSnapshots_FinancialGroups_FinancialGroupId",
                        column: x => x.FinancialGroupId,
                        principalTable: "FinancialGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PortfolioSnapshots_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SnapshotConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ChangeThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CheckIntervalHours = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnapshotConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnapshotConfigurations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioSnapshots_FinancialGroupId",
                table: "PortfolioSnapshots",
                column: "FinancialGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioSnapshots_UserId",
                table: "PortfolioSnapshots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapshotConfigurations_UserId",
                table: "SnapshotConfigurations",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioSnapshots");

            migrationBuilder.DropTable(
                name: "SnapshotConfigurations");
        }
    }
}

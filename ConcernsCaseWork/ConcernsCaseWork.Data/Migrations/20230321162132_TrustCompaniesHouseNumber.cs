using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrustCompaniesHouseNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrustCompaniesHouseNumber",
                schema: "concerns",
                table: "ConcernsCase",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "DecisionOutcomeBusinessArea",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "FinancialProviderMarketOversight");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrustCompaniesHouseNumber",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "DecisionOutcomeBusinessArea",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "ProviderMarketOversight");
        }
    }
}

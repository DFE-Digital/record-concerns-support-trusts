using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_To_DecisionOutcomeBusinessArea_Enum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "concerns",
                table: "DecisionOutcomeBusinessArea",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 7, "FinancialBusinessPartner" },
                    { 8, "EducationEstates" },
                    { 9, "FundingAndFinancialOversight" },
                    { 10, "SchoolDeliveryTeams" },
                    { 11, "FinancialGovernanceTeam" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "DecisionOutcomeBusinessArea",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "DecisionOutcomeBusinessArea",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "DecisionOutcomeBusinessArea",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "DecisionOutcomeBusinessArea",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "DecisionOutcomeBusinessArea",
                keyColumn: "Id",
                keyValue: 11);
        }
    }
}

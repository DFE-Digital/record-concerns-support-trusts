using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class DecisionAuthorizerText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "RegionalDirector");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "CounterSigningDeputyDirector");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "RegionalSchoolsCommissioner");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "CountersigningDeputyDirector");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_FinancialRecoveryOfFraudOrIrregularity_To_ConcernsDecisionTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsDecisionTypeId",
                columns: new[] { "Id", "Name" },
                values: new object[] { 12, "FinancialRecoveryOfFraudOrIrregularity" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsDecisionTypeId",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}

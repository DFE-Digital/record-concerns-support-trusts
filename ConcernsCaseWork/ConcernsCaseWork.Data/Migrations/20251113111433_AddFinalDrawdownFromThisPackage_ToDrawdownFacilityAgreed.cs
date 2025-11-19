using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalDrawdownFromThisPackage_ToDrawdownFacilityAgreed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsDecisionDrawdownFacilityAgreed",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "FinalDrawdownFromThisPackage" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsDecisionDrawdownFacilityAgreed",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}

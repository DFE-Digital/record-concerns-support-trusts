using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFinancialSupportPackageTypeEnumValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsDecisionDrawdownFacilityAgreed",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 5, "ANewPackageWithDrawdown" },
                    { 6, "ANewPackageWithImmediatePayment" },
                    { 7, "ADrawdownFromAnExistingPackage" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsDecisionDrawdownFacilityAgreed",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsDecisionDrawdownFacilityAgreed",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsDecisionDrawdownFacilityAgreed",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}

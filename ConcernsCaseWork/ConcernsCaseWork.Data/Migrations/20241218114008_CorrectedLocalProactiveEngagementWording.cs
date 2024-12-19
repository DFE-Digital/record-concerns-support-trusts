using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedLocalProactiveEngagementWording : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivity",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "LocalProactiveEngagement");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivityType",
                keyColumn: "Id",
                keyValue: 20,
                column: "Name",
                value: "LocalProactiveEngagement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivity",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "LocalProactiveEngagament");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivityType",
                keyColumn: "Id",
                keyValue: 20,
                column: "Name",
                value: "LocalProactiveEngagament");
        }
    }
}

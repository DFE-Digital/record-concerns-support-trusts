using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTTEOptionName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivityType",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "OtherNationalProcesses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivityType",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "OtherVulnerability");
        }
    }
}

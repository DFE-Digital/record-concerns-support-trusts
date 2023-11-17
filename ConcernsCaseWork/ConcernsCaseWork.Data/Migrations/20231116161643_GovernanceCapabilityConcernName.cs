using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class GovernanceCapabilityConcernName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "Governance capability");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "Governance");
        }
    }
}

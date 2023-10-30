using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSafeguardingDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 24,
                column: "Name",
                value: "Safeguarding non-compliance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 24,
                column: "Name",
                value: "Safeguarding");
        }
    }
}

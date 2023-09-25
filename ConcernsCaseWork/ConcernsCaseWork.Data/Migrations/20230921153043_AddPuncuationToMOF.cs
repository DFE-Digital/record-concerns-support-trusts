using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPuncuationToMOF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "ESFA activity, TFF or other departmental activity.");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "CIU casework, whistleblowing, self reported, regional director (RD) or other government bodies.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "ESFA activity, TFF or other departmental activity");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "CIU casework, whistleblowing, self reported, regional director (RD) or other government bodies");
        }
    }
}

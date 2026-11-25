using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Replace_SFSO_With_Trust_Regulation : Migration
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
                value: "Trust Regulation activity, TFF or other departmental activity.");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "Division",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Trust Regulation");
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
                value: "SFSO activity, TFF or other departmental activity.");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "Division",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "SFSO (Schools Financial Support and Oversight)");
        }
    }
}

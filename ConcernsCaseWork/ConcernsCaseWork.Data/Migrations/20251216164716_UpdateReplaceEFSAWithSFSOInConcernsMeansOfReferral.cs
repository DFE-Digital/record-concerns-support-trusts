using Microsoft.EntityFrameworkCore.Migrations;

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReplaceEFSAWithSFSOInConcernsMeansOfReferral : Migration
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
                value: "SFSO activity, TFF or other departmental activity.");
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
                value: "ESFA activity, TFF or other departmental activity.");
        }
    }
}

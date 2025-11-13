using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExceptionalAnnualGrantEAG_To_FrameworkCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsDecisionFrameworkCategory",
                columns: new[] { "Id", "Name" },
                values: new object[] { 6, "ExceptionalAnnualGrantEAG" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsDecisionFrameworkCategory",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}

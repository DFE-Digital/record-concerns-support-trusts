using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Some_Options_To_DecisionOutcomeAuthorizer_Enum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 9, "DirectorGeneral" },
                    { 10, "HMT" },
                    { 11, "FGT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                keyColumn: "Id",
                keyValue: 11);
        }
    }
}

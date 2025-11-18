using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_SecretaryOfState_To_DecisionOutcomeAuthorizer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                columns: new[] { "Id", "Name" },
                values: new object[] { 8, "SecretaryOfState" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}

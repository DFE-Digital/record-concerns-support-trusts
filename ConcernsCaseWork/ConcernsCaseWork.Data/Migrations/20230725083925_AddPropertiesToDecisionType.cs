using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertiesToDecisionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DecisionDrawdownFacilityAgreedId",
                schema: "concerns",
                table: "ConcernsDecisionType",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DecisionFrameworkCategoryId",
                schema: "concerns",
                table: "ConcernsDecisionType",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConcernsDecisionDrawdownFacilityAgreed",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConcernsDecisionDrawdownFacilityAgreed", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConcernsDecisionFrameworkCategory",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConcernsDecisionFrameworkCategory", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsDecisionDrawdownFacilityAgreed",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Yes" },
                    { 2, "No" },
                    { 3, "PaymentUnderExistingArrangement" }
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsDecisionFrameworkCategory",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "EnablingFinancialRecovery" },
                    { 2, "BuildingFinancialCapacity" },
                    { 3, "FacilitatingTransferFinanciallyTriggered" },
                    { 4, "FacilitatingTransferEducationallyTriggered" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConcernsDecisionDrawdownFacilityAgreed",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "ConcernsDecisionFrameworkCategory",
                schema: "concerns");

            migrationBuilder.DropColumn(
                name: "DecisionDrawdownFacilityAgreedId",
                schema: "concerns",
                table: "ConcernsDecisionType");

            migrationBuilder.DropColumn(
                name: "DecisionFrameworkCategoryId",
                schema: "concerns",
                table: "ConcernsDecisionType");
        }
    }
}

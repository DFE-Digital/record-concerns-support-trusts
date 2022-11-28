using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    public partial class AddDecisionOutcome : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DecisionOutcome",
                schema: "concerns",
                columns: table => new
                {
                    DecisionOutcomeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DecisionId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "money", nullable: true),
                    DecisionMadeDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DecisionEffectiveFromDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Authorizer = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionOutcome", x => x.DecisionOutcomeId);
                    table.ForeignKey(
                        name: "FK_DecisionOutcome_ConcernsDecision_DecisionId",
                        column: x => x.DecisionId,
                        principalSchema: "concerns",
                        principalTable: "ConcernsDecision",
                        principalColumn: "DecisionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DecisionOutcomeAuthorizer",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionOutcomeAuthorizer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DecisionOutcomeBusinessArea",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionOutcomeBusinessArea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DecisionOutcomeStatus",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionOutcomeStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DecisionOutcomeBusinessAreaMapping",
                schema: "concerns",
                columns: table => new
                {
                    DecisionOutcomeId = table.Column<int>(type: "int", nullable: false),
                    DecisionOutcomeBusinessId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionOutcomeBusinessAreaMapping", x => new { x.DecisionOutcomeId, x.DecisionOutcomeBusinessId });
                    table.ForeignKey(
                        name: "FK_DecisionOutcomeBusinessAreaMapping_DecisionOutcome_DecisionOutcomeId",
                        column: x => x.DecisionOutcomeId,
                        principalSchema: "concerns",
                        principalTable: "DecisionOutcome",
                        principalColumn: "DecisionOutcomeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "DecisionOutcomeAuthorizer",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "G7" },
                    { 2, "G6" },
                    { 3, "RegionalSchoolsCommissioner" },
                    { 4, "DeputyDirector" },
                    { 5, "CountersigningDeputyDirector" },
                    { 6, "Director" },
                    { 7, "Minister" }
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "DecisionOutcomeBusinessArea",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "SchoolsFinancialSupportAndOversight" },
                    { 2, "BusinessPartner" },
                    { 3, "Capital" },
                    { 4, "Funding" },
                    { 5, "ProviderMarketOversight" },
                    { 6, "RegionsGroup" }
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "DecisionOutcomeStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Approved" },
                    { 2, "ApprovedWithConditions" },
                    { 3, "PartiallyApproved" },
                    { 4, "Withdrawn" },
                    { 5, "Declined" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DecisionOutcome_DecisionId",
                schema: "concerns",
                table: "DecisionOutcome",
                column: "DecisionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DecisionOutcomeAuthorizer",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "DecisionOutcomeBusinessArea",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "DecisionOutcomeBusinessAreaMapping",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "DecisionOutcomeStatus",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "DecisionOutcome",
                schema: "concerns");
        }
    }
}

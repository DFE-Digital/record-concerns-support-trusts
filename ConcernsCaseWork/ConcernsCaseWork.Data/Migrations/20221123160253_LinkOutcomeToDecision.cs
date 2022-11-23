using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    public partial class LinkOutcomeToDecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DecisionOutcome_DecisionId",
                schema: "concerns",
                table: "DecisionOutcome",
                column: "DecisionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DecisionOutcome_ConcernsDecision_DecisionId",
                schema: "concerns",
                table: "DecisionOutcome",
                column: "DecisionId",
                principalSchema: "concerns",
                principalTable: "ConcernsDecision",
                principalColumn: "DecisionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DecisionOutcome_ConcernsDecision_DecisionId",
                schema: "concerns",
                table: "DecisionOutcome");

            migrationBuilder.DropIndex(
                name: "IX_DecisionOutcome_DecisionId",
                schema: "concerns",
                table: "DecisionOutcome");
        }
    }
}

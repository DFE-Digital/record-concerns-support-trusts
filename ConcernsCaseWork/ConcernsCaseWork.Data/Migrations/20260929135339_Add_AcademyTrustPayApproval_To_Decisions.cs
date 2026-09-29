using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_AcademyTrustPayApproval_To_Decisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DecisionAcademyTrustPayApprovalId",
                schema: "concerns",
                table: "ConcernsDecisionType",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConcernsDecisionAcademyTrustPayApproval",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConcernsDecisionAcademyTrustPayApproval", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsDecisionAcademyTrustPayApproval",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "RemunerationExceedingThreshold" },
                    { 2, "PerformanceRelatedExceedingThreshold" },
                    { 3, "IncreaseOfExecutivePayAtFasterRate" }
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsDecisionTypeId",
                columns: new[] { "Id", "Name" },
                values: new object[] { 13, "AcademyTrustPayApproval" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConcernsDecisionAcademyTrustPayApproval",
                schema: "concerns");

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsDecisionTypeId",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DropColumn(
                name: "DecisionAcademyTrustPayApprovalId",
                schema: "concerns",
                table: "ConcernsDecisionType");
        }
    }
}

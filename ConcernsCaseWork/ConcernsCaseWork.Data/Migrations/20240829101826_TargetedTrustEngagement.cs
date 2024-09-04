using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class TargetedTrustEngagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TargetedTrustEngagementActivity",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetedTrustEngagementActivity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetedTrustEngagementActivityType",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetedTrustEngagementActivityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetedTrustEngagementCase",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseUrn = table.Column<int>(type: "int", nullable: false),
                    EngagementStartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "VARCHAR(2000)", maxLength: 2000, nullable: true),
                    EngagementEndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EngagementOutcomeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetedTrustEngagementCase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetedTrustEngagementOutcome",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetedTrustEngagementOutcome", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetedTrustEngagementType",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    ActivityTypeId = table.Column<int>(type: "int", nullable: true),
                    TargetedTrustEngagementCaseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetedTrustEngagementType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetedTrustEngagementType_TargetedTrustEngagementCase_TargetedTrustEngagementCaseId",
                        column: x => x.TargetedTrustEngagementCaseId,
                        principalSchema: "concerns",
                        principalTable: "TargetedTrustEngagementCase",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivity",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "BudgetForecastReturnAccountsReturnDriven", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "ExecutivePayEngagement", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "FinancialReturnsAssurance", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "ReservesOversightAndAssuranceProject", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "LocalProactiveEngagament", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "OtherNationalProcesses", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "NoEngagementActivitiesWereTakenForward", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivityType",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Category1", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Category2", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Category3", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Category4", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "CEOs", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Leadership", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "CEOsAndLeadership", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "AnnualSummaryInternalScrutinyReports", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "AuditIssues", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "ManagementLetterIssues", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "RegularityIssues", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Priority1", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Priority2", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Priority3", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Priority4", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "BudgetForecastReturnAccountsReturnDriven", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "ExecutivePayEngagement", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "FinancialReturnsAssurance", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "ReservesOversightAssuranceProject", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "LocalProactiveEngagament", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "OtherVulnerability", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "TargetedTrustEngagementOutcome",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "AdequateResponseReceived", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "InadequateResponseReceived", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "NoEngagementTookPlace", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "NoResponseRequired", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TargetedTrustEngagementCase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "TargetedTrustEngagementCase",
                columns: new[] { "CaseUrn", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetedTrustEngagementType_TargetedTrustEngagementCaseId",
                schema: "concerns",
                table: "TargetedTrustEngagementType",
                column: "TargetedTrustEngagementCaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TargetedTrustEngagementActivity",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "TargetedTrustEngagementActivityType",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "TargetedTrustEngagementOutcome",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "TargetedTrustEngagementType",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "TargetedTrustEngagementCase",
                schema: "concerns");
        }
    }
}

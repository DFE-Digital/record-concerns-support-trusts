using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Adduniqueconstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConcernsRecord_CaseId",
                schema: "concerns",
                table: "ConcernsRecord");

            migrationBuilder.DropIndex(
                name: "IX_ConcernsDecision_ConcernsCaseId",
                schema: "concerns",
                table: "ConcernsDecision");

            migrationBuilder.AlterColumn<string>(
                name: "TrustUkprn",
                schema: "concerns",
                table: "ConcernsCase",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "concerns",
                table: "ConcernsCase",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrustFinancialForecast_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "TrustFinancialForecast",
                columns: new[] { "CaseUrn", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SRMACase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "SRMACase",
                columns: new[] { "CaseUrn", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NTIWarningLetterCase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "NTIWarningLetterCase",
                columns: new[] { "CaseUrn", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NTIUnderConsiderationCase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "NTIUnderConsiderationCase",
                columns: new[] { "CaseUrn", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NoticeToImproveCase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "NoticeToImproveCase",
                columns: new[] { "CaseUrn", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPlanCase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "FinancialPlanCase",
                columns: new[] { "CaseUrn", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsRecord_CaseId_CreatedAt",
                schema: "concerns",
                table: "ConcernsRecord",
                columns: new[] { "CaseId", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsDecision_ConcernsCaseId_CreatedAt",
                schema: "concerns",
                table: "ConcernsDecision",
                columns: new[] { "ConcernsCaseId", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsCase_TrustUkprn_CreatedAt_CreatedBy",
                schema: "concerns",
                table: "ConcernsCase",
                columns: new[] { "TrustUkprn", "CreatedAt", "CreatedBy" },
                unique: true,
                filter: "[TrustUkprn] IS NOT NULL AND [CreatedBy] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrustFinancialForecast_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "TrustFinancialForecast");

            migrationBuilder.DropIndex(
                name: "IX_SRMACase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "SRMACase");

            migrationBuilder.DropIndex(
                name: "IX_NTIWarningLetterCase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "NTIWarningLetterCase");

            migrationBuilder.DropIndex(
                name: "IX_NTIUnderConsiderationCase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "NTIUnderConsiderationCase");

            migrationBuilder.DropIndex(
                name: "IX_NoticeToImproveCase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "NoticeToImproveCase");

            migrationBuilder.DropIndex(
                name: "IX_FinancialPlanCase_CaseUrn_CreatedAt",
                schema: "concerns",
                table: "FinancialPlanCase");

            migrationBuilder.DropIndex(
                name: "IX_ConcernsRecord_CaseId_CreatedAt",
                schema: "concerns",
                table: "ConcernsRecord");

            migrationBuilder.DropIndex(
                name: "IX_ConcernsDecision_ConcernsCaseId_CreatedAt",
                schema: "concerns",
                table: "ConcernsDecision");

            migrationBuilder.DropIndex(
                name: "IX_ConcernsCase_TrustUkprn_CreatedAt_CreatedBy",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.AlterColumn<string>(
                name: "TrustUkprn",
                schema: "concerns",
                table: "ConcernsCase",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "concerns",
                table: "ConcernsCase",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsRecord_CaseId",
                schema: "concerns",
                table: "ConcernsRecord",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsDecision_ConcernsCaseId",
                schema: "concerns",
                table: "ConcernsDecision",
                column: "ConcernsCaseId");
        }
    }
}

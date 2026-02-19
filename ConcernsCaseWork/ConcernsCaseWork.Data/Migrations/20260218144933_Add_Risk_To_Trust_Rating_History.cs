using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Risk_To_Trust_Rating_History : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConcernsCaseRiskToTrustRatingHistory",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    RatingId = table.Column<int>(type: "int", nullable: false),
                    RationalCommentary = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RiskToTrustRatingHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsCaseRiskToTrustRatingHistory_CaseId",
                schema: "concerns",
                table: "ConcernsCaseRiskToTrustRatingHistory",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsCaseRiskToTrustRatingHistory_CreatedAt",
                schema: "concerns",
                table: "ConcernsCaseRiskToTrustRatingHistory",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConcernsCaseRiskToTrustRatingHistory",
                schema: "concerns");
        }
    }
}

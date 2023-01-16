using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrustFinancialForecast : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrustFinancialForecast",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseUrn = table.Column<int>(type: "int", nullable: false),
                    SRMAOfferedAfterTFF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ForecastingToolRanAt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WasTrustResponseSatisfactory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SFSOInitialReviewHappenedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TrustRespondedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TrustFinancialForecast", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrustFinancialForecast",
                schema: "concerns");
        }
    }
}

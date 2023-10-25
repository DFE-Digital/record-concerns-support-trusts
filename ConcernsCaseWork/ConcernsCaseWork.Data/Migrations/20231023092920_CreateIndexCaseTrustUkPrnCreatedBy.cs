using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateIndexCaseTrustUkPrnCreatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ConcernsCase_CreatedBy",
                schema: "concerns",
                table: "ConcernsCase",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsCase_TrustUkprn",
                schema: "concerns",
                table: "ConcernsCase",
                column: "TrustUkprn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConcernsCase_CreatedBy",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.DropIndex(
                name: "IX_ConcernsCase_TrustUkprn",
                schema: "concerns",
                table: "ConcernsCase");
        }
    }
}

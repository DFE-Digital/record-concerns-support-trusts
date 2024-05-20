using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class MeansOfReferralWhistleblowing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "CIU casework, self reported, regional director (RD) or other government bodies.");

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[] { 3, new DateTime(2024, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Whistleblowing", "Whistleblowing", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "CIU casework, whistleblowing, self reported, regional director (RD) or other government bodies.");
        }
    }
}

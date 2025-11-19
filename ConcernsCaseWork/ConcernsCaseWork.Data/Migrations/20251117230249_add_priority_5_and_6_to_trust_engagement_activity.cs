using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_priority_5_and_6_to_trust_engagement_activity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivityType",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 22, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Priority5", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 23, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Priority6", new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivityType",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "TargetedTrustEngagementActivityType",
                keyColumn: "Id",
                keyValue: 23);
        }
    }
}

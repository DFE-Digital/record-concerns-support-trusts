using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_New_Concern_types : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsType",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 27, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Actual and/or projected deficit", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 28, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Actual and/or projected cash shortfall", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 29, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Trust Closure", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 30, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Financial management/ATH compliance", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 31, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Late financial returns", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 32, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Irregularity and/or self-reported fraud", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 32);
        }
    }
}

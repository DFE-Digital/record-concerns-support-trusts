using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateconcernstypestomatchDaRT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "Projected deficit");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 8,
                column: "Name",
                value: "Governance and compliance");

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsType",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 20, new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Viability", "Financial", new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21, new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Irregularity", "Irregularity", new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 22, new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Suspected fraud", "Irregularity", new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 23, new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Compliance", "Governance and compliance", new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "Projected deficit / Low future surplus");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 8,
                column: "Name",
                value: "Governance");

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "ConcernsType",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Financial reporting", "Compliance", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Financial returns", "Compliance", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cash flow shortfall", "Financial", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Clawback", "Financial", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Closure", "Governance", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Executive Pay", "Governance", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Safeguarding", "Governance", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Allegations and self reported concerns", "Irregularity", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Related party transactions - in year", "Irregularity", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }
    }
}

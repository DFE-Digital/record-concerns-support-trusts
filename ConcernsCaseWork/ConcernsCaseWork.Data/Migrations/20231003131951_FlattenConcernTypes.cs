using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class FlattenConcernTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { null, "Deficit", new DateTime(2023, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { null, "Projected deficit", new DateTime(2023, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2023, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { null, "Financial governance", new DateTime(2023, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { null, "Viability", new DateTime(2023, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Description", "UpdatedAt" },
                values: new object[] { null, new DateTime(2023, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { null, "Suspected fraud", new DateTime(2023, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { null, "Financial compliance", new DateTime(2023, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "SRMAStatus",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Cancelled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { "Deficit", "Financial", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { "Projected deficit", "Financial", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { "Governance", "Governance and compliance", new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { "Viability", "Financial", new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Description", "UpdatedAt" },
                values: new object[] { "Irregularity", new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { "Suspected fraud", "Irregularity", new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Description", "Name", "UpdatedAt" },
                values: new object[] { "Compliance", "Governance and compliance", new DateTime(2022, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "SRMAStatus",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Canceled");
        }
    }
}

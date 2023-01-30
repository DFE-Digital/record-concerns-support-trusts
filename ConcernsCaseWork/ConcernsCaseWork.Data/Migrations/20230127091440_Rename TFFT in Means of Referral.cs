using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTFFTinMeansofReferral : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "UpdatedAt" },
                values: new object[] { "ESFA activity, TFF or other departmental activity", new DateTime(2023, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "UpdatedAt" },
                values: new object[] { "ESFA activity, TFFT or other departmental activity", new DateTime(2022, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}

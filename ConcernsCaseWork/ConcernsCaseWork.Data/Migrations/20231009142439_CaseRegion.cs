using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class CaseRegion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                schema: "concerns",
                table: "ConcernsCase",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Region",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CRegion__C5B214360AF620234", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "Region",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "East Midlands", new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "East of England", new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "London", new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "North East", new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "North West", new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "South East", new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "South West", new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "West Midlands", new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Yorkshire and The Humber", new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsCase_RegionId",
                schema: "concerns",
                table: "ConcernsCase",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConcernsCase_Region_RegionId",
                schema: "concerns",
                table: "ConcernsCase",
                column: "RegionId",
                principalSchema: "concerns",
                principalTable: "Region",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConcernsCase_Region_RegionId",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.DropTable(
                name: "Region",
                schema: "concerns");

            migrationBuilder.DropIndex(
                name: "IX_ConcernsCase_RegionId",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.DropColumn(
                name: "RegionId",
                schema: "concerns",
                table: "ConcernsCase");
        }
    }
}

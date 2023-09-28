using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDivisionToCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionFK",
                schema: "concerns",
                table: "ConcernsCase",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Division",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CDivision__C5B214360AF620234", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "Division",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "SFSO (Schools Financial Support and Oversight)", new DateTime(2023, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2023, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Regions Group", new DateTime(2023, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsCase_DivisionFK",
                schema: "concerns",
                table: "ConcernsCase",
                column: "DivisionFK");

            migrationBuilder.AddForeignKey(
                name: "FK_ConcernsCase_Division_DivisionFK",
                schema: "concerns",
                table: "ConcernsCase",
                column: "DivisionFK",
                principalSchema: "concerns",
                principalTable: "Division",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConcernsCase_Division_DivisionFK",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.DropTable(
                name: "Division",
                schema: "concerns");

            migrationBuilder.DropIndex(
                name: "IX_ConcernsCase_DivisionFK",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.DropColumn(
                name: "DivisionFK",
                schema: "concerns",
                table: "ConcernsCase");
        }
    }
}

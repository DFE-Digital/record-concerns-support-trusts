using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseLastUpdatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CaseLastUpdatedAt",
                schema: "concerns",
                table: "ConcernsCase",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseLastUpdatedAt",
                schema: "concerns",
                table: "ConcernsCase");
        }
    }
}

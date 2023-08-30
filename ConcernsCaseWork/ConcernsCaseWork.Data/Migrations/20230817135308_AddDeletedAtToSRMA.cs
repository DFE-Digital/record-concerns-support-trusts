using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedAtToSRMA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "concerns",
                table: "SRMACase",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsDecisionFrameworkCategory",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "BuildingFinancialCapability");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsDecisionFrameworkCategory",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "FacilitatingTransferFinanciallyAgreed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "concerns",
                table: "SRMACase");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsDecisionFrameworkCategory",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "BuildingFinancialCapacity");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsDecisionFrameworkCategory",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "FacilitatingTransferFinanciallyTriggered");
        }
    }
}

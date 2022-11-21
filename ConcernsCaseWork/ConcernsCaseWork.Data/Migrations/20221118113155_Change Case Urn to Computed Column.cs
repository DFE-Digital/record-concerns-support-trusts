using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    public partial class ChangeCaseUrntoComputedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsCase",
                type: "int",
                nullable: false,
                computedColumnSql: "[Id]",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");
            
            migrationBuilder.DropSequence(
                name: "GlobalSequence",
                schema: "concerns");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "GlobalSequence",
                schema: "concerns",
                minValue: 1L);

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsCase",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldComputedColumnSql: "[Id]");
        }
    }
}

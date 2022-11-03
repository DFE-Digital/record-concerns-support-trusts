using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    public partial class useidsratherthanurns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Concerns Record Status Urn
            migrationBuilder.AddColumn<int>(
	            name: "StatusId",
	            schema: "concerns",
	            table: "ConcernsRecord",
	            nullable: false,
	            defaultValue: 0);
            
            migrationBuilder.Sql("UPDATE [concerns].[ConcernsRecord] SET StatusId = (SELECT Id FROM [concerns].[ConcernsStatus] WHERE Urn = StatusUrn)");
            
            migrationBuilder.DropColumn(
	            name: "StatusUrn",
	            schema: "concerns",
	            table: "ConcernsRecord");
            // End Concerns Record Status Urn
            
            // Concerns Case Status Urn
            migrationBuilder.AddColumn<int>(
	            name: "StatusId",
	            schema: "concerns",
	            table: "ConcernsCase",
	            nullable: false,
	            defaultValue: 0);
            
            migrationBuilder.Sql("UPDATE [concerns].[ConcernsCase] SET StatusId = (SELECT Id FROM [concerns].[ConcernsStatus] WHERE Urn = StatusUrn)");
          
            migrationBuilder.DropColumn(
	            name: "StatusUrn",
	            schema: "concerns",
	            table: "ConcernsCase");
            // End Concerns Case Status Urn

            // Concerns Case Rating Urn
            migrationBuilder.AddColumn<int>(
	            name: "RatingId",
	            schema: "concerns",
	            table: "ConcernsCase",
	            nullable: false,
	            defaultValue: 0);
            migrationBuilder.Sql("UPDATE [concerns].[ConcernsCase] SET RatingId = (SELECT Id FROM [concerns].[ConcernsRating] WHERE Urn = RatingUrn)");
            migrationBuilder.DropColumn(
	            name: "RatingUrn",
	            schema: "concerns",
	            table: "ConcernsCase");
            // End Concerns Case Status Urn
            
            migrationBuilder.DropColumn(
	            name: "Urn",
	            schema: "concerns",
	            table: "SRMAStatus");

            migrationBuilder.DropColumn(
	            name: "Urn",
	            schema: "concerns",
	            table: "SRMAReason");

            migrationBuilder.DropColumn(
	            name: "Urn",
	            schema: "concerns",
	            table: "SRMACase");

            migrationBuilder.DropColumn(
	            name: "Urn",
	            schema: "concerns",
	            table: "ConcernsType");

            migrationBuilder.DropColumn(
	            name: "Urn",
	            schema: "concerns",
	            table: "ConcernsStatus");

            migrationBuilder.DropColumn(
	            name: "Urn",
	            schema: "concerns",
	            table: "ConcernsRecord");

            migrationBuilder.DropColumn(
	            name: "Urn",
	            schema: "concerns",
	            table: "ConcernsRating");

            migrationBuilder.DropColumn(
	            name: "Urn",
	            schema: "concerns",
	            table: "ConcernsMeansOfReferral");

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsRecord_StatusId",
                schema: "concerns",
                table: "ConcernsRecord",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsCase_RatingId",
                schema: "concerns",
                table: "ConcernsCase",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsCase_StatusId",
                schema: "concerns",
                table: "ConcernsCase",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConcernsCase_ConcernsRating_RatingId",
                schema: "concerns",
                table: "ConcernsCase",
                column: "RatingId",
                principalSchema: "concerns",
                principalTable: "ConcernsRating",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ConcernsCase_ConcernsStatus_StatusId",
                schema: "concerns",
                table: "ConcernsCase",
                column: "StatusId",
                principalSchema: "concerns",
                principalTable: "ConcernsStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ConcernsRecord_ConcernsStatus_StatusId",
                schema: "concerns",
                table: "ConcernsRecord",
                column: "StatusId",
                principalSchema: "concerns",
                principalTable: "ConcernsStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConcernsCase_ConcernsRating_RatingId",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.DropForeignKey(
                name: "FK_ConcernsCase_ConcernsStatus_StatusId",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.DropForeignKey(
                name: "FK_ConcernsRecord_ConcernsStatus_StatusId",
                schema: "concerns",
                table: "ConcernsRecord");

            migrationBuilder.DropIndex(
                name: "IX_ConcernsRecord_StatusId",
                schema: "concerns",
                table: "ConcernsRecord");

            migrationBuilder.DropIndex(
                name: "IX_ConcernsCase_RatingId",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.DropIndex(
                name: "IX_ConcernsCase_StatusId",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                schema: "concerns",
                table: "ConcernsRecord",
                newName: "StatusUrn");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                schema: "concerns",
                table: "ConcernsCase",
                newName: "StatusUrn");

            migrationBuilder.RenameColumn(
                name: "RatingId",
                schema: "concerns",
                table: "ConcernsCase",
                newName: "RatingUrn");

            migrationBuilder.AddColumn<long>(
                name: "Urn",
                schema: "concerns",
                table: "SRMAStatus",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Urn",
                schema: "concerns",
                table: "SRMAReason",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Urn",
                schema: "concerns",
                table: "SRMACase",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsType",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");

            migrationBuilder.AddColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsStatus",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");

            migrationBuilder.AddColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsRecord",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");

            migrationBuilder.AddColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsRating",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");

            migrationBuilder.AddColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");
        }
    }
}

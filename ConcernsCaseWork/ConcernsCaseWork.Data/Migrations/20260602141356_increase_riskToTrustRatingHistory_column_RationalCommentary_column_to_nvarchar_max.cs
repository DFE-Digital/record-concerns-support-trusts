using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class increase_riskToTrustRatingHistory_column_RationalCommentary_column_to_nvarchar_max : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.AlterColumn<string>(
				name: "RationalCommentary",
				schema: "concerns",
				table: "ConcernsCaseRiskToTrustRatingHistory",
				type: "nvarchar(max)",
				nullable: true, // Changes column to allow NULLs
				oldClrType: typeof(string),
				oldType: "nvarchar(250)",
				oldMaxLength: 250);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			// Reverts the column back to its original state if you roll back
			migrationBuilder.AlterColumn<string>(
				name: "RationalCommentary",
				schema: "concerns",
				table: "ConcernsCaseRiskToTrustRatingHistory",
				type: "nvarchar(250)",
				maxLength: 250,
				nullable: false, // Reverts back to NOT NULL
				oldClrType: typeof(string),
				oldType: "nvarchar(max)",
				oldNullable: true);
		}
    }
}

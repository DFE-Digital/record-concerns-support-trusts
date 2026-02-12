using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Case_Rational : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RatingRational",
                schema: "concerns",
                table: "ConcernsCase",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RatingRationalCommentary",
                schema: "concerns",
                table: "ConcernsCase",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingRational",
                schema: "concerns",
                table: "ConcernsCase");

            migrationBuilder.DropColumn(
                name: "RatingRationalCommentary",
                schema: "concerns",
                table: "ConcernsCase");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class UkPrnFieldLength12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TrustUkprn",
                schema: "concerns",
                table: "ConcernsCase",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TrustUkprn",
                schema: "concerns",
                table: "ConcernsCase",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12,
                oldNullable: true);
        }
    }
}

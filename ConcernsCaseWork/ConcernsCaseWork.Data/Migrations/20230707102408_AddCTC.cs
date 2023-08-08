using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCTC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CityTechnologyCollege",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UKPRN = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    CompaniesHouseNumber = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AddressLine3 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Town = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    County = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityTechnologyCollege", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CityTechnologyCollege",
                schema: "concerns");
        }
    }
}

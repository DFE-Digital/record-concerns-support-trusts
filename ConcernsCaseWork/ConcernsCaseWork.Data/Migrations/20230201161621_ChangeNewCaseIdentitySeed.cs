using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNewCaseIdentitySeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			// All new cases start with a 2000000
			// Cases that were migrated from Dart start with 1000000
			// Ensures we have no conflicts if we decide to migrate more DaRT cases in the future
			migrationBuilder.Sql("DBCC CHECKIDENT ([concerns.ConcernsCase], RESEED, 2000000)");

		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			// No point in resetting the seed, would cause more problems if we did it wrong
        }
    }
}

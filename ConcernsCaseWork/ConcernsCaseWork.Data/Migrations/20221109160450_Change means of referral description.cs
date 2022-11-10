using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    public partial class Changemeansofreferraldescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(
		        @"UPDATE [concerns].[ConcernsMeansOfReferral] 
					SET [Description] = 'CIU casework, whistleblowing, self reported, regional director (RD) or other government bodies',
					    [UpdatedAt] = '2022-11-09'
					WHERE Id = 2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(
		        @"UPDATE [concerns].[ConcernsMeansOfReferral] 
					SET [Description] = 'CIU casework, whistleblowing, self reported, RSCs or other government bodies',
					    [UpdatedAt] = '2022-07-28'
					WHERE Id = 2");
        }
    }
}

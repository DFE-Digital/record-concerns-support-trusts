using ConcernsCaseWork.UserContext;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;


namespace ConcernsCaseWork.Data
{
	[ExcludeFromCodeCoverage]
	public class ConcernsDbContextFactory : IDesignTimeDbContextFactory<ConcernsDbContext>
	{
		public ConcernsDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<ConcernsDbContext>();

			var configPath = Path.Combine(Directory.GetCurrentDirectory(), "../ConcernsCaseWork/appsettings.json");

			var configuration = new ConfigurationBuilder()
				.AddJsonFile(configPath)
				.Build();

			var userInfoService = new ServerUserInfoService()
			{
				UserInfo = new UserInfo() { Name = "Design.Time@test.gov.uk", Roles = new[] { Claims.CaseWorkerRoleClaim } }
			};

			var connectionString = configuration.GetConnectionString("DefaultConnection");
			optionsBuilder.UseConcernsSqlServer(connectionString);

			var interceptors = new List<IInterceptor>();

			return new ConcernsDbContext(optionsBuilder.Options, userInfoService, interceptors);
		}
	}
}

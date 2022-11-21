using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests;

[TestFixture]
public class PocTestUsingSqlLite
{
	[Test]
	public void PocExampleTestUsingSqlLite()
	{
		// arrange
		var connection = new SqliteConnection("DataSource=:memory:");
		connection.Open();
 
		var options = new DbContextOptionsBuilder<ConcernsDbContext>().UseSqlite(connection).Options;
 
		using (var context = new ConcernsDbContext(options))
		{
			context.Database.EnsureCreated();
		}
 
		var meansOfReferral = new ConcernsMeansOfReferral { Name = "test", Description = "test", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
		using (var context = new ConcernsDbContext(options))
		{
			context.ConcernsMeansOfReferrals.Add(meansOfReferral);
			context.SaveChanges();
		}
 
		using (var context = new ConcernsDbContext(options))
		{
			var sut = new ConcernsMeansOfReferralGateway(context);
				
			// act
			var result = sut.GetMeansOfReferralById(meansOfReferral.Id);
				
			// assert
			result.Description.Should().Be(meansOfReferral.Description);
		}
	}
}
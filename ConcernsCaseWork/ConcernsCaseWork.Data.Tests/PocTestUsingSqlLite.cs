using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;

namespace ConcernsCaseWork.Data.Tests;

[TestFixture]
public class ConcernsCaseTriggerTests : DatabaseFixtureBase
{
	public ConcernsCaseTriggerTests()
	{
		var meansOfReferral = new ConcernsMeansOfReferral { Name = "test", Description = "test", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
        using (var context = GetNewConcernsDbContext())
        {
        	context.ConcernsMeansOfReferrals.Add(meansOfReferral);
        	context.SaveChanges();
        }
	}
	
	[Test]
	public void PocExampleTestUsingSqlLite()
	{
		// arrange
		var id = 3;

		using var context = GetNewConcernsDbContext();
		var sut = new ConcernsMeansOfReferralGateway(context);
				
		// act
		var result = sut.GetMeansOfReferralById(id);
				
		// assert
		result.Description.Should().Be("test");
	}
	
	[Test]
	public void PocExampleTestUsingSqlLite2()
	{
		// arrange
		var id = 4;

		using var context = GetNewConcernsDbContext();
		var sut = new ConcernsMeansOfReferralGateway(context);
				
		// act
		var result = sut.GetMeansOfReferralById(id);
				
		// assert
		result.Description.Should().Be("test");
	}
}
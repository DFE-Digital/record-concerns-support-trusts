using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Tests.TestData;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests.DbGateways;

public class TestCaseDbGateway : DatabaseTestFixture
{
	protected readonly TestDataFactory _testDataFactory = new ();
	
	public ConcernsCase GenerateTestOpenCase()
	{
		var statusId = GetDefaultCaseStatus().Id;
		var ratingId = GetDefaultCaseRating().Id;

		var concernsCase = _testDataFactory.BuildOpenCase(statusId, ratingId);

		return AddCase(concernsCase);
	}

	public ConcernsCase AddCase(ConcernsCase concernsCase)
	{
		using var context = CreateContext();
		context.ConcernsCase.Add(concernsCase);
		context.SaveChanges();
		
		return GetCase(concernsCase.Id);
	}
			
	public ConcernsCase UpdateCase(ConcernsCase concernsCase)
	{
		using var context = CreateContext();
		context.ConcernsCase.Update(concernsCase);
		context.SaveChanges();
		return concernsCase;
	}
	
	public ConcernsCase GetCase(int id)
	{
		using var context = CreateContext();
		
		return context.ConcernsCase
			.Where(x => x.Id == id)
			.Include(x => x.Rating)
			.Include(x => x.Status)
			.Single();
	}
	
	public ConcernsStatus GetDefaultCaseStatus()
	{
		using var context = CreateContext();
		return context.ConcernsStatus.First();
	}

	public ConcernsRating GetDefaultCaseRating()
	{
		using var context = CreateContext();
		return context.ConcernsRatings.First();
	}
	
	public ConcernsRating GetDifferentCaseRating(int currentId)
	{
		using var context = CreateContext();
		return context.ConcernsRatings.First(x => x.Id != currentId);
	}
}
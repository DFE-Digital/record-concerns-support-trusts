using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests.DbGateways;

public class TestConcernDbGateway : TestCaseDbGateway
{
	public ConcernsRecord GenerateTestConcernInDb()
	{
		var parentCase = GenerateTestOpenCase();
		
		var statusId = GetDefaultCaseStatus().Id;
		var ratingId = GetDefaultCaseRating().Id;
		var typeId = GetDefaultConcernType().Id;
		
		var concern = _testDataFactory.BuildOpenConcern(parentCase.Id, statusId, ratingId, typeId);

		return AddConcern(concern);
	}
	
	public ConcernsRecord AddConcern(ConcernsRecord concern)
	{
		using var context = CreateContext();
		context.ConcernsRecord.Add(concern);
		context.SaveChanges();
        
		return GetConcern(concern.Id);
	}
	
	public ConcernsRecord UpdateConcern(ConcernsRecord concern)
	{
		using var context = CreateContext();
		context.ConcernsRecord.Update(concern);
		context.SaveChanges();

		return GetConcern(concern.Id);
	}

	public ConcernsRecord GetConcern(int id)
	{
		using var context = CreateContext();
		
		return context.ConcernsRecord
			.Where(x => x.Id == id)
			.Include(x => x.ConcernsRating)
			.Include(x => x.ConcernsType)
			.Include(x => x.Status)
			.Single();
	}
	
	public ConcernsType GetDefaultConcernType()
	{
		using var context = CreateContext();
		return context.ConcernsTypes.First();
	}
}
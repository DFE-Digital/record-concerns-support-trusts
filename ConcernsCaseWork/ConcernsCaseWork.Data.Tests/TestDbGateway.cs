using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests;

public class TestDbGateway : DatabaseTestFixture
{
	private readonly TestDataFactory _testDataFactory = new ();
	
	public ConcernsCase GenerateTestOpenCaseInDb()
	{
		var statusId = GetDefaultStatus().Id;
		var ratingId = GetDefaultRating().Id;

		var concernsCase = _testDataFactory.BuildOpenCase(statusId, ratingId);

		return AddCase(concernsCase);
	}

	public ConcernsRecord GenerateTestConcernInDb()
	{
		var parentCase = GenerateTestOpenCaseInDb();
		
		var statusId = GetDefaultStatus().Id;
		var ratingId = GetDefaultRating().Id;
		var typeId = GetDefaultType().Id;
		
		var concern = _testDataFactory.BuildOpenConcern(parentCase.Id, statusId, ratingId, typeId);

		return AddConcern(concern);
	}

	public ConcernsCase AddCase(ConcernsCase concernsCase)
	{
		using var context = CreateContext();
		context.ConcernsCase.Add(concernsCase);
		context.SaveChanges();
		
		return GetCaseById(concernsCase.Id);
	}
	
	public ConcernsRecord AddConcern(ConcernsRecord concern)
	{
		using var context = CreateContext();
        context.ConcernsRecord.Add(concern);
        context.SaveChanges();
        
        return GetConcernById(concern.Id);
	}
	
	public ConcernsRecord UpdateConcern(ConcernsRecord concern)
	{
		using var context = CreateContext();
		context.ConcernsRecord.Update(concern);
		context.SaveChanges();

		return GetConcernById(concern.Id);
	}

	private ConcernsRecord GetConcernById(int id)
	{
		using var context = CreateContext();
		
		return context.ConcernsRecord
        	.Where(c => c.Id == id)
        	.Include(c => c.ConcernsRating)
        	.Include(c => c.ConcernsType)
        	.Include(c => c.Status)
        	.Single();
	}
	
	private ConcernsCase GetCaseById(int id)
	{
		using var context = CreateContext();
		
		return context.ConcernsCase
			.Where(c => c.Id == id)
			.Include(c => c.Rating)
			.Include(c => c.Status)
			.Single();
	}
		
	public ConcernsCase UpdateCase(ConcernsCase concernsCase)
	{
		using var context = CreateContext();
		context.ConcernsCase.Update(concernsCase);
		context.SaveChanges();
		return concernsCase;
	}
	
	public ConcernsRating GetDifferentRating(int currentId)
	{
		using var context = CreateContext();
		return context.ConcernsRatings.First(r => r.Id != currentId);
	}
	
	

	private ConcernsStatus GetDefaultStatus()
	{
		using var context = CreateContext();
		return context.ConcernsStatus.First();
	}
	
	private ConcernsRating GetDefaultRating()
	{
		using var context = CreateContext();
		return context.ConcernsRatings.First();
	}
	
	private ConcernsType GetDefaultType()
	{
		using var context = CreateContext();
		return context.ConcernsTypes.First();
	}
}
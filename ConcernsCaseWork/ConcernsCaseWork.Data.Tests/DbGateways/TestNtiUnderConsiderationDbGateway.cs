using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests.DbGateways;

public class TestNtiUnderConsiderationDbGateway : TestCaseDbGateway
{
	public NTIUnderConsideration GenerateTestNtiUnderConsiderationInDb()
	{
		var parentCase = GenerateTestOpenCase();

		var ntiUnderConsideration = _testDataFactory.BuildOpenNtiUnderConsideration(parentCase.Id);

		using var context = CreateContext();
		context.NTIUnderConsiderations.Add(ntiUnderConsideration);
		context.SaveChanges();

		return GetNtiUnderConsiderationCase(ntiUnderConsideration.Id);
	}
	
	public NTIUnderConsideration UpdateNtiUnderConsideration(NTIUnderConsideration ntiUnderConsideration)
	{
		using var context = CreateContext();
		context.NTIUnderConsiderations.Update(ntiUnderConsideration);
		context.SaveChanges();

		return GetNtiUnderConsiderationCase(ntiUnderConsideration.Id);
	}
	
	public NTIUnderConsideration GetNtiUnderConsiderationCase(long ntiUnderConsiderationId)
	{
		using var context = CreateContext();
		return context
			.NTIUnderConsiderations
			.Include(x => x.ClosedStatus)
			.Single(x => x.Id == ntiUnderConsiderationId);
	}

	public NTIUnderConsiderationStatus GetDefaultClosedStatus()
	{
		using var context = CreateContext();
		return context.NTIUnderConsiderationStatuses.First();
	}
}
using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests.DbGateways;

public class TestSrmaDbGateway : TestCaseDbGateway
{
	public SRMACase GenerateTestSrma()
	{
		var parentCase = GenerateTestOpenCase();

		var srma = _testDataFactory.BuildOpenSrmaCase(parentCase.Id, GetDefaultStatus().Id);

		using var context = CreateContext();
		context.SRMACases.Add(srma);
		context.SaveChanges();

		return GetSrmaCase(srma.Id);
	}

	public SRMACase UpdateSrma(SRMACase srma)
	{
		using var context = CreateContext();
		context.SRMACases.Update(srma);
		context.SaveChanges();

		return GetSrmaCase(srma.Id);
	}

	public SRMACase GetSrmaCase(long srmaId)
	{
		using var context = CreateContext();
		return context
			.SRMACases
			.Single(x => x.Id == srmaId);
	}
	
	public SRMAStatus GetDifferentStatus(long? currentId)
	{
		using var context = CreateContext();
		return context.SRMAStatuses.First(x => x.Id != currentId);
	}
	
	public SRMAStatus GetStatusById(long? statusId)
	{
		using var context = CreateContext();
		return context.SRMAStatuses.Single(x => x.Id == statusId);
	}
	
	public SRMAStatus GetDefaultStatus()
	{
		using var context = CreateContext();
		return context.SRMAStatuses.First();
	}
}
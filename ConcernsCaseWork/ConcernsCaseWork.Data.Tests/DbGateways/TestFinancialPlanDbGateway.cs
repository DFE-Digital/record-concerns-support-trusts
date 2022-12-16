using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests.DbGateways;

public class TestFinancialPlanDbGateway : TestCaseDbGateway
{
	public FinancialPlanCase GenerateTestFinancialPlanInDb()
	{
		var parentCase = GenerateTestOpenCase();

		var financialPlan = _testDataFactory.BuildOpenFinancialPlan(parentCase.Id, (int)GetDefaultStatus().Id);

		using var context = CreateContext();
		context.FinancialPlanCases.Add(financialPlan);
		context.SaveChanges();

		return GetFinancialPlanCase(financialPlan.Id);
	}

	public FinancialPlanCase UpdateFinancialPlan(FinancialPlanCase financialPlan)
	{
		using var context = CreateContext();
		context.FinancialPlanCases.Update(financialPlan);
		context.SaveChanges();

		return GetFinancialPlanCase(financialPlan.Id);
	}

	public FinancialPlanCase GetFinancialPlanCase(long financialPlanId)
	{
		using var context = CreateContext();
		return context
			.FinancialPlanCases
			.Where(x => x.Id == financialPlanId)
			.Include(x => x.Status)
			.Single();
	}

	public FinancialPlanStatus GetDefaultStatus()
	{
		using var context = CreateContext();
		return context.FinancialPlanStatuses.First();
	}
		
	public FinancialPlanStatus GetDifferentStatus(long? currentId)
	{
		using var context = CreateContext();
		return context.FinancialPlanStatuses.First(x => x.Id != currentId);
	}
}
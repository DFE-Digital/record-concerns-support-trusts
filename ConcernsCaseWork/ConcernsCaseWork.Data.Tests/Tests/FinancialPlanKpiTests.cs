using ConcernsCaseWork.Data.Tests.DbGateways;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConcernsCaseWork.Data.Tests.Tests;

[TestFixture]
public class FinancialPlanKpiTests : DatabaseTestFixture
{
	private readonly RandomGenerator _randomGenerator = new ();
	private readonly TestFinancialPlanDbGateway _gateway = new ();

	[Test]
	public void CreateNewFinancialPlan_CreatesKpiEntries()
	{
		// act
		var financialPlan = _gateway.GenerateTestFinancialPlanInDb();
		
		// assert
		financialPlan.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(financialPlan.CaseUrn, 0);

		var createdAtKpi = results.Single(r => r.DataItemChanged == "CreatedAt");
		createdAtKpi.ActionId.Should().Be(financialPlan.Id);
		createdAtKpi.ActionType.Should().Be("FinancialPlanCase");
		createdAtKpi.DateTimeOfChange.Should().Be(financialPlan.UpdatedAt);
		createdAtKpi.DataItemChanged.Should().Be("CreatedAt");
		createdAtKpi.Operation.Should().Be("Create");
		createdAtKpi.OldValue.Should().BeEmpty();
		createdAtKpi.NewValue.Replace("-","/").Should().Be(financialPlan.CreatedAt.ToShortDateString());

		var statusKpi = results.Single(r => r.DataItemChanged == "Status");
		statusKpi.ActionId.Should().Be(financialPlan.Id);
		statusKpi.ActionType.Should().Be("FinancialPlanCase");
		statusKpi.DateTimeOfChange.Should().Be(financialPlan.UpdatedAt);
		statusKpi.DataItemChanged.Should().Be("Status");
		statusKpi.Operation.Should().Be("Update");
		statusKpi.OldValue.Should().BeEmpty();
		statusKpi.NewValue.Should().Be(financialPlan.Status.Name);

		results.Count.Should().Be(2);
	}

	[Test]
	public void UpdateFinancialPlan_Status_CreatesKpiEntry()
	{
		// arrange
		var financialPlan = _gateway.GenerateTestFinancialPlanInDb();
		var originalStatus = financialPlan.Status.Name;
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(financialPlan.CaseUrn);
		
		// act
		financialPlan.Status = _gateway.GetDifferentStatus(financialPlan.StatusId);
		financialPlan.UpdatedAt = _randomGenerator.DateTime();
		_gateway.UpdateFinancialPlan(financialPlan);
		
		// assert
		var results = GetKpiResults(financialPlan.CaseUrn, maxKpiIdAfterCreate);

		var statusKpi = results.Single(r => r.DataItemChanged == "Status");
		statusKpi.ActionId.Should().Be(financialPlan.Id);
		statusKpi.ActionType.Should().Be("FinancialPlanCase");
		statusKpi.DateTimeOfChange.Should().Be(financialPlan.UpdatedAt);
		statusKpi.DataItemChanged.Should().Be("Status");
		statusKpi.Operation.Should().Be("Update");
		statusKpi.OldValue.Should().Be(originalStatus);
		statusKpi.NewValue.Should().Be(financialPlan.Status.Name);

		results.Count.Should().Be(1);
	}
	
	[Test]
	public void CloseFinancialPlan_CreatesKpiEntryForClosedAt()
	{
		// arrange
		var financialPlan = _gateway.GenerateTestFinancialPlanInDb();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(financialPlan.CaseUrn);
		
		// act
		financialPlan.UpdatedAt = _randomGenerator.DateTime();
		financialPlan.ClosedAt = _randomGenerator.DateTime();
		_gateway.UpdateFinancialPlan(financialPlan);
		
		// assert
		var results = GetKpiResults(financialPlan.CaseUrn, maxKpiIdAfterCreate);

		var closedAtKpi = results.Single(r => r.DataItemChanged == "ClosedAt");
		closedAtKpi.ActionId.Should().Be(financialPlan.Id);
		closedAtKpi.ActionType.Should().Be("FinancialPlanCase");
		closedAtKpi.DateTimeOfChange.Should().Be(financialPlan.UpdatedAt);
		closedAtKpi.DataItemChanged.Should().Be("ClosedAt");
		closedAtKpi.Operation.Should().Be("Close");
		closedAtKpi.OldValue.Should().BeEmpty();
		closedAtKpi.NewValue.Replace("-","/").Should().Be(financialPlan.ClosedAt!.Value.ToShortDateString());

		results.Count.Should().Be(1);
	}
	
	private List<ActionKpi> GetKpiResults(long caseUrn, int previousMaxKpiId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = 
			"SELECT ActionType, ActionId, DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [concerns].[kpi-CaseAction] WHERE CaseUrn = @Id AND Id > @PreviousMaxKpiId";
		command.Parameters.Add(new SqlParameter("Id", caseUrn));
		command.Parameters.Add(new SqlParameter("PreviousMaxKpiId", previousMaxKpiId));
		
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		var kpis = new List<ActionKpi>();
		while (result.Read())
		{
			kpis.Add(BuildActionKpi(result));
		}

		return kpis;
	}
	
	private int GetMaxKpiIdForCase(int caseUrn)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = "SELECT MAX(Id) FROM [concerns].[kpi-CaseAction] WHERE CaseUrn = @Id";
		command.Parameters.Add(new SqlParameter("Id", caseUrn));
		
		context.Database.OpenConnection();
		var maxKpiId = command.ExecuteScalar() ?? 0;

		return (int)maxKpiId;
	}

	private static ActionKpi BuildActionKpi(IDataRecord record) 
		=> new (record.GetString(0), record.GetInt64(1), record.GetDateTime(2), record.GetString(3), record.GetString(4), record.GetString(5), record.GetString(6));
	
	private record ActionKpi(string ActionType, long ActionId, DateTime DateTimeOfChange, string DataItemChanged, string Operation, string OldValue, string NewValue);
}
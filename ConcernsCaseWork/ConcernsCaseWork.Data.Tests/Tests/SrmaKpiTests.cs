using ConcernsCaseWork.Data.Tests.DbGateways;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConcernsCaseWork.Data.Tests.Tests;

[TestFixture]
public class SrmaKpiTests : DatabaseTestFixture
{
	private readonly RandomGenerator _randomGenerator = new ();
	private readonly TestSrmaDbGateway _gateway = new ();

	[Test]
	public void CreateNewSrma_CreatesKpiEntries()
	{
		// act
		var srma = _gateway.GenerateTestSrma();
		
		// assert
		srma.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(srma.CaseUrn, srma.Id, 0);

		var createdAtKpi = results.Single(r => r.DataItemChanged == "CreatedAt");
		createdAtKpi.ActionId.Should().Be(srma.Id);
		createdAtKpi.ActionType.Should().Be("SRMACase");
		createdAtKpi.DateTimeOfChange.Should().Be(srma.UpdatedAt);
		createdAtKpi.DataItemChanged.Should().Be("CreatedAt");
		createdAtKpi.Operation.Should().Be("Create");
		createdAtKpi.OldValue.Should().BeEmpty();
		createdAtKpi.NewValue.Replace("-","/").Should().Be(srma.CreatedAt.ToShortDateString());

		var statusKpi = results.Single(r => r.DataItemChanged == "Status");
		statusKpi.ActionId.Should().Be(srma.Id);
		statusKpi.ActionType.Should().Be("SRMACase");
		statusKpi.DateTimeOfChange.Should().Be(srma.UpdatedAt);
		statusKpi.DataItemChanged.Should().Be("Status");
		statusKpi.Operation.Should().Be("Update");
		statusKpi.OldValue.Should().BeEmpty();
		statusKpi.NewValue.Should().Be(_gateway.GetStatusById(srma.StatusId).Name);

		results.Count.Should().Be(2);
	}

	[Test]
	public void UpdateSrma_Status_CreatesKpiEntry()
	{
		// arrange
		var srma = _gateway.GenerateTestSrma();
		var originalStatus = _gateway.GetStatusById(srma.StatusId).Name;
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(srma.CaseUrn, srma.Id);
		
		// act
		srma.StatusId = _gateway.GetDifferentStatus(srma.StatusId).Id;
		srma.UpdatedAt = _randomGenerator.DateTime();
		_gateway.UpdateSrma(srma);
		
		// assert
		var results = GetKpiResults(srma.CaseUrn, srma.Id, maxKpiIdAfterCreate);

		var statusKpi = results.Single(r => r.DataItemChanged == "Status");
		statusKpi.ActionId.Should().Be(srma.Id);
		statusKpi.ActionType.Should().Be("SRMACase");
		statusKpi.DateTimeOfChange.Should().Be(srma.UpdatedAt);
		statusKpi.DataItemChanged.Should().Be("Status");
		statusKpi.Operation.Should().Be("Update");
		statusKpi.OldValue.Should().Be(originalStatus);
		statusKpi.NewValue.Should().Be(_gateway.GetStatusById(srma.StatusId).Name);

		results.Count.Should().Be(1);
	}
	
	[Test]
	public void CloseSrma_CreatesKpiEntryForClosedAt()
	{
		// arrange
		var srma = _gateway.GenerateTestSrma();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(srma.CaseUrn, srma.Id);
		
		// act
		srma.UpdatedAt = _randomGenerator.DateTime();
		srma.ClosedAt = _randomGenerator.DateTime();
		_gateway.UpdateSrma(srma);
		
		// assert
		var results = GetKpiResults(srma.CaseUrn, srma.Id, maxKpiIdAfterCreate);

		var closedAtKpi = results.Single(r => r.DataItemChanged == "ClosedAt");
		closedAtKpi.ActionId.Should().Be(srma.Id);
		closedAtKpi.ActionType.Should().Be("SRMACase");
		closedAtKpi.DateTimeOfChange.Should().Be(srma.UpdatedAt);
		closedAtKpi.DataItemChanged.Should().Be("ClosedAt");
		closedAtKpi.Operation.Should().Be("Close");
		closedAtKpi.OldValue.Should().BeEmpty();
		closedAtKpi.NewValue.Replace("-","/").Should().Be(srma.ClosedAt!.Value.ToShortDateString());

		results.Count.Should().Be(1);
	}
	
	private List<ActionKpi> GetKpiResults(long caseUrn, int srmaId, int previousMaxKpiId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = 
			"SELECT ActionType, ActionId, DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [kpi].[ConcernsCaseAction] WHERE CaseUrn = @Id AND ActionType='SRMACase' AND ActionId=@SrmaId AND Id > @PreviousMaxKpiId";
		command.Parameters.Add(new SqlParameter("Id", caseUrn));
		command.Parameters.Add(new SqlParameter("PreviousMaxKpiId", previousMaxKpiId));
		command.Parameters.Add(new SqlParameter("SrmaId", srmaId));
		
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		var kpis = new List<ActionKpi>();
		while (result.Read())
		{
			kpis.Add(BuildActionKpi(result));
		}

		return kpis;
	}
	
	private int GetMaxKpiIdForCase(int caseUrn, int srmaId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = "SELECT MAX(Id) FROM [kpi].[ConcernsCaseAction] WHERE CaseUrn = @Id AND ActionType='SRMACase' AND ActionId=@SrmaId";
		command.Parameters.Add(new SqlParameter("Id", caseUrn));
		command.Parameters.Add(new SqlParameter("SrmaId", srmaId));
		
		context.Database.OpenConnection();
		var maxKpiId = command.ExecuteScalar() ?? 0;

		return (int)maxKpiId;
	}

	private static ActionKpi BuildActionKpi(IDataRecord record) 
		=> new (record.GetString(0), record.GetInt64(1), record.GetDateTime(2), record.GetString(3), record.GetString(4), record.GetString(5), record.GetString(6));
	
	private record ActionKpi(string ActionType, long ActionId, DateTime DateTimeOfChange, string DataItemChanged, string Operation, string OldValue, string NewValue);
}
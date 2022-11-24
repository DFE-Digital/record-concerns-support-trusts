using ConcernsCaseWork.Data.Tests.DbGateways;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConcernsCaseWork.Data.Tests.Tests;

[TestFixture]
public class NtiUnderConsiderationKpiTests : DatabaseTestFixture
{
	private readonly RandomGenerator _randomGenerator = new ();
	private readonly TestNtiUnderConsiderationDbGateway _gateway = new ();

	[Test]
	public void CreateNewNtiUnderConsideration_CreatesKpiEntries()
	{
		// act
		var ntiUnderConsideration = _gateway.GenerateTestNtiUnderConsiderationInDb();
		
		// assert
		ntiUnderConsideration.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(ntiUnderConsideration.CaseUrn, ntiUnderConsideration.Id, 0);

		var createdAtKpi = results.Single(r => r.DataItemChanged == "CreatedAt");
		createdAtKpi.ActionId.Should().Be(ntiUnderConsideration.Id);
		createdAtKpi.ActionType.Should().Be("NTIUnderConsiderationCase");
		createdAtKpi.DateTimeOfChange.Should().Be(ntiUnderConsideration.UpdatedAt);
		createdAtKpi.DataItemChanged.Should().Be("CreatedAt");
		createdAtKpi.Operation.Should().Be("Create");
		createdAtKpi.OldValue.Should().BeEmpty();
		createdAtKpi.NewValue.Replace("-","/").Should().Be(ntiUnderConsideration.CreatedAt.ToShortDateString());

		results.Count.Should().Be(1);
	}

	[Test]
	public void CloseNtiUnderConsideration_CreatesKpiEntryForClosedAtAndClosedStatus()
	{
		// arrange
		var ntiUnderConsideration = _gateway.GenerateTestNtiUnderConsiderationInDb();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(ntiUnderConsideration.CaseUrn, ntiUnderConsideration.Id);
		
		// act
		ntiUnderConsideration.UpdatedAt = _randomGenerator.DateTime();
		ntiUnderConsideration.ClosedAt = _randomGenerator.DateTime();
		ntiUnderConsideration.ClosedStatusId = _gateway.GetDefaultClosedStatus().Id;
		var updatedNti = _gateway.UpdateNtiUnderConsideration(ntiUnderConsideration);
		
		// assert
		var results = GetKpiResults(updatedNti.CaseUrn, updatedNti.Id, maxKpiIdAfterCreate);

		var closedAtKpi = results.Single(r => r.DataItemChanged == "ClosedAt");
		closedAtKpi.ActionId.Should().Be(updatedNti.Id);
		closedAtKpi.ActionType.Should().Be("NTIUnderConsiderationCase");
		closedAtKpi.DateTimeOfChange.Should().Be(updatedNti.UpdatedAt);
		closedAtKpi.DataItemChanged.Should().Be("ClosedAt");
		closedAtKpi.Operation.Should().Be("Close");
		closedAtKpi.OldValue.Should().BeEmpty();
		closedAtKpi.NewValue.Replace("-","/").Should().Be(updatedNti.ClosedAt!.Value.ToShortDateString());
		
		var closedStatusKpi = results.Single(r => r.DataItemChanged == "Status");
		closedStatusKpi.ActionId.Should().Be(updatedNti.Id);
		closedStatusKpi.ActionType.Should().Be("NTIUnderConsiderationCase");
		closedStatusKpi.DateTimeOfChange.Should().Be(updatedNti.UpdatedAt);
		closedStatusKpi.DataItemChanged.Should().Be("Status");
		closedStatusKpi.Operation.Should().Be("Update");
		closedStatusKpi.OldValue.Should().BeEmpty();
		closedStatusKpi.NewValue.Should().Be(updatedNti.ClosedStatus.Name);

		results.Count.Should().Be(2);
	}
	
	private List<ActionKpi> GetKpiResults(long caseUrn, long ntiUnderConsiderationId, int previousMaxKpiId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = 
			@"SELECT ActionType, ActionId, DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue 
				FROM [concerns].[kpi-CaseAction] 
				WHERE CaseUrn = @Id AND ActionType='NTIUnderConsiderationCase' AND ActionId=@NtiUnderConsiderationId AND Id > @PreviousMaxKpiId";
		command.Parameters.Add(new SqlParameter("Id", caseUrn));
		command.Parameters.Add(new SqlParameter("PreviousMaxKpiId", previousMaxKpiId));
		command.Parameters.Add(new SqlParameter("NtiUnderConsiderationId", ntiUnderConsiderationId));
		
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		var kpis = new List<ActionKpi>();
		while (result.Read())
		{
			kpis.Add(BuildActionKpi(result));
		}

		return kpis;
	}
	
	private int GetMaxKpiIdForCase(int caseUrn, long ntiUnderConsiderationId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = @"SELECT MAX(Id) FROM [concerns].[kpi-CaseAction] 
			WHERE CaseUrn = @Id AND ActionType='NtiUnderConsiderationCase' AND ActionId=@NtiUnderConsiderationId";
		command.Parameters.Add(new SqlParameter("Id", caseUrn));
		command.Parameters.Add(new SqlParameter("NtiUnderConsiderationId", ntiUnderConsiderationId));
		
		context.Database.OpenConnection();
		var maxKpiId = command.ExecuteScalar() ?? 0;

		return (int)maxKpiId;
	}

	private static ActionKpi BuildActionKpi(IDataRecord record) 
		=> new (record.GetString(0), record.GetInt64(1), record.GetDateTime(2), record.GetString(3), record.GetString(4), record.GetString(5), record.GetString(6));
	
	private record ActionKpi(string ActionType, long ActionId, DateTime DateTimeOfChange, string DataItemChanged, string Operation, string OldValue, string NewValue);
}
using ConcernsCaseWork.Data.Tests.DbGateways;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConcernsCaseWork.Data.Tests.Tests;

[TestFixture]
public class NtiWarningLetterKpiTests : DatabaseTestFixture
{
	private readonly RandomGenerator _randomGenerator = new ();
	private readonly TestNtiWarningLetterDbGateway _gateway = new ();

	[Test]
	public void CreateNewNtiWarningLetter_CreatesKpiEntries()
	{
		// act
		var ntiWarningLetter = _gateway.GenerateTestNtiWarningLetter();
		
		// assert
		ntiWarningLetter.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(ntiWarningLetter.CaseUrn, ntiWarningLetter.Id, 0);

		var createdAtKpi = results.Single(r => r.DataItemChanged == "CreatedAt");
		createdAtKpi.ActionId.Should().Be(ntiWarningLetter.Id);
		createdAtKpi.ActionType.Should().Be("NTIWarningLetterCase");
		createdAtKpi.DateTimeOfChange.Should().Be(ntiWarningLetter.UpdatedAt);
		createdAtKpi.DataItemChanged.Should().Be("CreatedAt");
		createdAtKpi.Operation.Should().Be("Create");
		createdAtKpi.OldValue.Should().BeEmpty();
		createdAtKpi.NewValue.Replace("-","/").Should().Be(ntiWarningLetter.CreatedAt.ToShortDateString());
		
		var statusKpi = results.Single(r => r.DataItemChanged == "Status");
		statusKpi.ActionId.Should().Be(ntiWarningLetter.Id);
		statusKpi.ActionType.Should().Be("NTIWarningLetterCase");
		statusKpi.DateTimeOfChange.Should().Be(ntiWarningLetter.UpdatedAt);
		statusKpi.DataItemChanged.Should().Be("Status");
		statusKpi.Operation.Should().Be("Update");
		statusKpi.OldValue.Should().BeEmpty();
		statusKpi.NewValue.Should().Be(ntiWarningLetter.Status.Name);

		results.Count.Should().Be(2);
	}
	
	[Test]
	public void UpdateNtiWarningLetter_ChangeStatus_CreatesKpiEntry()
	{
		// arrange
		var ntiWarningLetter = _gateway.GenerateTestNtiWarningLetter();
		var originalStatus = ntiWarningLetter.Status.Name;
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(ntiWarningLetter.CaseUrn, ntiWarningLetter.Id);
		
		// act
		ntiWarningLetter.UpdatedAt = _randomGenerator.DateTime();
		ntiWarningLetter.Status = _gateway.GetDifferentNonClosedStatus(ntiWarningLetter.StatusId);
		var updatedNti = _gateway.UpdateNtiWarningLetter(ntiWarningLetter);
		
		// assert
		var results = GetKpiResults(updatedNti.CaseUrn, updatedNti.Id, maxKpiIdAfterCreate);
		
		var statusKpi = results.Single(r => r.DataItemChanged == "Status");
		statusKpi.ActionId.Should().Be(updatedNti.Id);
		statusKpi.ActionType.Should().Be("NTIWarningLetterCase");
		statusKpi.DateTimeOfChange.Should().Be(updatedNti.UpdatedAt);
		statusKpi.DataItemChanged.Should().Be("Status");
		statusKpi.Operation.Should().Be("Update");
		statusKpi.OldValue.Should().Be(originalStatus);
		statusKpi.NewValue.Should().Be(updatedNti.Status.Name);

		results.Count.Should().Be(1);
	}

	[Test]
	public void CloseNtiWarningLetter_CreatesKpiEntryForClosedAtAndClosedStatus()
	{
		// arrange
		var ntiWarningLetter = _gateway.GenerateTestNtiWarningLetter();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(ntiWarningLetter.CaseUrn, ntiWarningLetter.Id);
		
		// act
		ntiWarningLetter.UpdatedAt = _randomGenerator.DateTime();
		ntiWarningLetter.ClosedAt = _randomGenerator.DateTime();
		ntiWarningLetter.ClosedStatusId = _gateway.GetDefaultClosedStatus().Id;
		var updatedNti = _gateway.UpdateNtiWarningLetter(ntiWarningLetter);
		
		// assert
		var results = GetKpiResults(updatedNti.CaseUrn, updatedNti.Id, maxKpiIdAfterCreate);

		var closedAtKpi = results.Single(r => r.DataItemChanged == "ClosedAt");
		closedAtKpi.ActionId.Should().Be(updatedNti.Id);
		closedAtKpi.ActionType.Should().Be("NTIWarningLetterCase");
		closedAtKpi.DateTimeOfChange.Should().Be(updatedNti.UpdatedAt);
		closedAtKpi.DataItemChanged.Should().Be("ClosedAt");
		closedAtKpi.Operation.Should().Be("Close");
		closedAtKpi.OldValue.Should().BeEmpty();
		closedAtKpi.NewValue.Replace("-","/").Should().Be(updatedNti.ClosedAt!.Value.ToShortDateString());
		
		var closedStatusKpi = results.Single(r => r.DataItemChanged == "ClosedStatus");
		closedStatusKpi.ActionId.Should().Be(updatedNti.Id);
		closedStatusKpi.ActionType.Should().Be("NTIWarningLetterCase");
		closedStatusKpi.DateTimeOfChange.Should().Be(updatedNti.UpdatedAt);
		closedStatusKpi.DataItemChanged.Should().Be("ClosedStatus");
		closedStatusKpi.Operation.Should().Be("Update");
		closedStatusKpi.OldValue.Should().BeEmpty();
		closedStatusKpi.NewValue.Should().Be(updatedNti.ClosedStatus.Name);

		results.Count.Should().Be(2);
	}
	
	private List<ActionKpi> GetKpiResults(long caseUrn, long ntiWarningLetterId, int previousMaxKpiId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = 
			@"SELECT ActionType, ActionId, DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue 
				FROM [kpi].[ConcernsCaseAction] 
				WHERE CaseUrn = @Id AND ActionType='NTIWarningLetterCase' AND ActionId=@NtiWarningLetterId AND Id > @PreviousMaxKpiId";
		command.Parameters.Add(new SqlParameter("Id", caseUrn));
		command.Parameters.Add(new SqlParameter("PreviousMaxKpiId", previousMaxKpiId));
		command.Parameters.Add(new SqlParameter("NtiWarningLetterId", ntiWarningLetterId));
		
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		var kpis = new List<ActionKpi>();
		while (result.Read())
		{
			kpis.Add(BuildActionKpi(result));
		}

		return kpis;
	}
	
	private int GetMaxKpiIdForCase(int caseUrn, long ntiWarningLetterId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = @"SELECT MAX(Id) FROM [kpi].[ConcernsCaseAction] 
			WHERE CaseUrn = @Id AND ActionType='NtiWarningLetterCase' AND ActionId=@NtiWarningLetterId";
		command.Parameters.Add(new SqlParameter("Id", caseUrn));
		command.Parameters.Add(new SqlParameter("NtiWarningLetterId", ntiWarningLetterId));
		
		context.Database.OpenConnection();
		var maxKpiId = command.ExecuteScalar() ?? 0;

		return (int)maxKpiId;
	}

	private static ActionKpi BuildActionKpi(IDataRecord record) 
		=> new (record.GetString(0), record.GetInt64(1), record.GetDateTime(2), record.GetString(3), record.GetString(4), record.GetString(5), record.GetString(6));
	
	private record ActionKpi(string ActionType, long ActionId, DateTime DateTimeOfChange, string DataItemChanged, string Operation, string OldValue, string NewValue);
}
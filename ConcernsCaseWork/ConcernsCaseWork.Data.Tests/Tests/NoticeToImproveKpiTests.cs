using ConcernsCaseWork.Data.Tests.DbGateways;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConcernsCaseWork.Data.Tests.Tests;

[TestFixture]
[Ignore("These tests currently must be run manually as they depend on removing and re-creating a database instance which is not yet supported in our pipelines")]
public class NoticeToImproveKpiTests : DatabaseTestFixture
{
	private readonly RandomGenerator _randomGenerator = new ();
	private readonly TestNoticeToImproveDbGateway _gateway = new ();

	[Test]
	public void CreateNewNoticeToImprove_CreatesKpiEntries()
	{
		// act
		var noticeToImprove = _gateway.GenerateTestNoticeToImproveInDb();
		
		// assert
		noticeToImprove.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(noticeToImprove.CaseUrn, noticeToImprove.Id, 0);

		var createdAtKpi = results.Single(r => r.DataItemChanged == "CreatedAt");
		createdAtKpi.ActionId.Should().Be(noticeToImprove.Id);
		createdAtKpi.ActionType.Should().Be("NoticeToImproveCase");
		createdAtKpi.DateTimeOfChange.Should().Be(noticeToImprove.UpdatedAt);
		createdAtKpi.DataItemChanged.Should().Be("CreatedAt");
		createdAtKpi.Operation.Should().Be("Create");
		createdAtKpi.OldValue.Should().BeEmpty();
		createdAtKpi.NewValue.Replace("-","/").Should().Be(noticeToImprove.CreatedAt.ToShortDateString());

		var statusKpi = results.Single(r => r.DataItemChanged == "Status");
		statusKpi.ActionId.Should().Be(noticeToImprove.Id);
		statusKpi.ActionType.Should().Be("NoticeToImproveCase");
		statusKpi.DateTimeOfChange.Should().Be(noticeToImprove.UpdatedAt);
		statusKpi.DataItemChanged.Should().Be("Status");
		statusKpi.Operation.Should().Be("Update");
		statusKpi.OldValue.Should().BeEmpty();
		statusKpi.NewValue.Should().Be(noticeToImprove.Status.Name);

		results.Count.Should().Be(2);
	}

	[Test]
	public void UpdateNoticeToImprove_Status_CreatesKpiEntry()
	{
		// arrange
		var noticeToImprove = _gateway.GenerateTestNoticeToImproveInDb();
		var originalStatus = noticeToImprove.Status.Name;
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(noticeToImprove.CaseUrn, noticeToImprove.Id);
		
		// act
		noticeToImprove.Status = _gateway.GetDifferentStatus(noticeToImprove.StatusId);
		noticeToImprove.UpdatedAt = _randomGenerator.DateTime();
		_gateway.UpdateNoticeToImprove(noticeToImprove);
		
		// assert
		var results = GetKpiResults(noticeToImprove.CaseUrn, noticeToImprove.Id, maxKpiIdAfterCreate);

		var statusKpi = results.Single(r => r.DataItemChanged == "Status");
		statusKpi.ActionId.Should().Be(noticeToImprove.Id);
		statusKpi.ActionType.Should().Be("NoticeToImproveCase");
		statusKpi.DateTimeOfChange.Should().Be(noticeToImprove.UpdatedAt);
		statusKpi.DataItemChanged.Should().Be("Status");
		statusKpi.Operation.Should().Be("Update");
		statusKpi.OldValue.Should().Be(originalStatus);
		statusKpi.NewValue.Should().Be(noticeToImprove.Status.Name);

		results.Count.Should().Be(1);
	}
	
	[Test]
	public void CloseNoticeToImprove_CreatesKpiEntryForClosedAt()
	{
		// arrange
		var noticeToImprove = _gateway.GenerateTestNoticeToImproveInDb();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(noticeToImprove.CaseUrn, noticeToImprove.Id);
		
		// act
		noticeToImprove.UpdatedAt = _randomGenerator.DateTime();
		noticeToImprove.ClosedAt = _randomGenerator.DateTime();
		_gateway.UpdateNoticeToImprove(noticeToImprove);
		
		// assert
		var results = GetKpiResults(noticeToImprove.CaseUrn, noticeToImprove.Id, maxKpiIdAfterCreate);

		var closedAtKpi = results.Single(r => r.DataItemChanged == "ClosedAt");
		closedAtKpi.ActionId.Should().Be(noticeToImprove.Id);
		closedAtKpi.ActionType.Should().Be("NoticeToImproveCase");
		closedAtKpi.DateTimeOfChange.Should().Be(noticeToImprove.UpdatedAt);
		closedAtKpi.DataItemChanged.Should().Be("ClosedAt");
		closedAtKpi.Operation.Should().Be("Close");
		closedAtKpi.OldValue.Should().BeEmpty();
		closedAtKpi.NewValue.Replace("-","/").Should().Be(noticeToImprove.ClosedAt!.Value.ToShortDateString());

		results.Count.Should().Be(1);
	}
	
	private List<ActionKpi> GetKpiResults(long caseUrn, long noticeToImproveId, int previousMaxKpiId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = 
			"SELECT ActionType, ActionId, DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [concerns].[kpi-CaseAction] WHERE CaseUrn = @Id AND ActionType='NoticeToImproveCase' AND ActionId=@NoticeToImproveId AND Id > @PreviousMaxKpiId";
		command.Parameters.Add(new SqlParameter("Id", caseUrn));
		command.Parameters.Add(new SqlParameter("PreviousMaxKpiId", previousMaxKpiId));
		command.Parameters.Add(new SqlParameter("NoticeToImproveId", noticeToImproveId));
		
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		var kpis = new List<ActionKpi>();
		while (result.Read())
		{
			kpis.Add(BuildActionKpi(result));
		}

		return kpis;
	}
	
	private int GetMaxKpiIdForCase(int caseUrn, long noticeToImproveId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = "SELECT MAX(Id) FROM [concerns].[kpi-CaseAction] WHERE CaseUrn = @Id AND ActionType='NoticeToImproveCase' AND ActionId=@NoticeToImproveId";
		command.Parameters.Add(new SqlParameter("Id", caseUrn));
		command.Parameters.Add(new SqlParameter("NoticeToImproveId", noticeToImproveId));
		
		context.Database.OpenConnection();
		var maxKpiId = command.ExecuteScalar() ?? 0;

		return (int)maxKpiId;
	}

	private static ActionKpi BuildActionKpi(IDataRecord record) 
		=> new (record.GetString(0), record.GetInt64(1), record.GetDateTime(2), record.GetString(3), record.GetString(4), record.GetString(5), record.GetString(6));
	
	private record ActionKpi(string ActionType, long ActionId, DateTime DateTimeOfChange, string DataItemChanged, string Operation, string OldValue, string NewValue);
}
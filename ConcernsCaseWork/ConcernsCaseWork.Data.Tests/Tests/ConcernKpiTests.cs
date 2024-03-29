using ConcernsCaseWork.Data.Tests.DbGateways;
using ConcernsCaseWork.Data.Tests.Helpers;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConcernsCaseWork.Data.Tests.Tests;

[TestFixture]
public class ConcernKpiTests : DatabaseTestFixture
{
	private readonly RandomGenerator _randomGenerator = new ();
	private readonly TestConcernDbGateway _gateway = new ();

	[Test]
	public void CreateNewConcern_CreatesKpiEntries()
	{
		// act
		var concern = _gateway.GenerateTestConcernInDb();

		// assert
		concern.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(concern.Id, 0);

		var createdAtKpi = results.Single(r => r.DataItemChanged == "CreatedAt");
		createdAtKpi.CaseId.Should().Be(concern.CaseId);
		createdAtKpi.DateTimeOfChange.Should().Be(concern.UpdatedAt);
		createdAtKpi.DataItemChanged.Should().Be("CreatedAt");
		createdAtKpi.Operation.Should().Be("Create");
		createdAtKpi.OldValue.Should().BeEmpty();
		createdAtKpi.NewValue.Should().Be(concern.CreatedAt.ToKpiDateFormat());
		
		var riskKpi = results.Single(r => r.DataItemChanged == "Risk");
		riskKpi.CaseId.Should().Be(concern.CaseId);
		riskKpi.DateTimeOfChange.Should().Be(concern.UpdatedAt);
		riskKpi.DataItemChanged.Should().Be("Risk");
		riskKpi.Operation.Should().Be("Update");
		riskKpi.OldValue.Should().BeEmpty();
		riskKpi.NewValue.Should().Be(concern.ConcernsRating.Name);

		results.Count.Should().Be(2);
	}
	
	[Test]
	public void UpdateConcern_RiskToTrust_CreatesKpiEntry()
	{
		// arrange
		var concern = _gateway.GenerateTestConcernInDb();
		var maxKpiIdAfterCreate = GetMaxKpiIdForConcern(concern.Id);
		var originalRating = concern.ConcernsRating.Name;
		
		// act
		concern.ConcernsRating = _gateway.GetDifferentCaseRating(concern.RatingId);
		concern.RatingId = concern.ConcernsRating.Id;
		_gateway.UpdateConcern(concern);

		// assert
		concern.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(concern.Id, maxKpiIdAfterCreate);

		var riskKpi = results.Single(r => r.DataItemChanged == "Risk");
		riskKpi.CaseId.Should().Be(concern.CaseId);
		riskKpi.DateTimeOfChange.Should().Be(concern.UpdatedAt);
		riskKpi.DataItemChanged.Should().Be("Risk");
		riskKpi.Operation.Should().Be("Update");
		riskKpi.OldValue.Should().Be(originalRating);
		riskKpi.NewValue.Should().Be(concern.ConcernsRating.Name);

		results.Count.Should().Be(1);
	}
	
	[Test]
	public void CloseConcern_CreatesKpiEntry()
	{
		// arrange
		var concern = _gateway.GenerateTestConcernInDb();
		var maxKpiIdAfterCreate = GetMaxKpiIdForConcern(concern.Id);
		
		// act
		concern.ClosedAt = _randomGenerator.DateTime();
		concern.UpdatedAt = _randomGenerator.DateTime();
		_gateway.UpdateConcern(concern);

		// assert
		concern.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(concern.Id, maxKpiIdAfterCreate);

		var closedAtKpi = results.Single(r => r.DataItemChanged == "ClosedAt");
		closedAtKpi.CaseId.Should().Be(concern.CaseId);
		closedAtKpi.DateTimeOfChange.Should().Be(concern.UpdatedAt);
		closedAtKpi.DataItemChanged.Should().Be("ClosedAt");
		closedAtKpi.Operation.Should().Be("Close");
		closedAtKpi.OldValue.Should().BeEmpty();
		closedAtKpi.NewValue.Should().Be(concern.ClosedAt.Value.ToKpiDateFormat());

		results.Count.Should().Be(1);
	}

	private static ConcernKpi BuildConcernKpi(IDataRecord record)
		=> new (record.GetInt32(0), record.GetDateTime(1), record.GetString(2), record.GetString(3), record.GetString(4), record.GetString(5));

	private List<ConcernKpi> GetKpiResults(int concernId, int previousMaxKpiId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = 
			"SELECT CaseId, DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [concerns].[kpi-Concern] WHERE RecordId = @Id AND Id > @PreviousMaxKpiId";
		command.Parameters.Add(new SqlParameter("Id", concernId));
		command.Parameters.Add(new SqlParameter("PreviousMaxKpiId", previousMaxKpiId));
		
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		var kpis = new List<ConcernKpi>();
		while (result.Read())
		{
			kpis.Add(BuildConcernKpi(result));
		}

		return kpis;
	}
		
	private int GetMaxKpiIdForConcern(int concernId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = "SELECT MAX(Id) FROM [concerns].[kpi-Concern] WHERE RecordId = @Id";
		command.Parameters.Add(new SqlParameter("Id", concernId));
		
		context.Database.OpenConnection();
		var maxKpiId = command.ExecuteScalar() ?? 0;

		return (int)maxKpiId;
	}
	
	private record ConcernKpi(int CaseId, DateTime DateTimeOfChange, string DataItemChanged, string Operation, string OldValue, string NewValue);
}
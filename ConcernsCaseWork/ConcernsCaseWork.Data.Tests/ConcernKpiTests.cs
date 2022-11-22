using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConcernsCaseWork.Data.Tests;

[TestFixture]
public class ConcernKpiTests : DatabaseTestFixture
{
	private readonly RandomGenerator _randomGenerator = new ();
	private readonly TestDataFactory _testDataFactory = new ();

	[Test]
	public void CreateNewConcern_CreatesKpiEntries()
	{
		// arrange
		var start = DateTime.Now;

		// act
		var concern = CreateConcernInDb();

		// assert
		concern.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(concern.Id, start);

		var createdAtKpi = results.Single(r => r.DataItemChanged == "CreatedAt");
		createdAtKpi.CaseId.Should().Be(concern.CaseId);
		createdAtKpi.DateTimeOfChange.Should().Be(concern.UpdatedAt);
		createdAtKpi.DataItemChanged.Should().Be("CreatedAt");
		createdAtKpi.Operation.Should().Be("Create");
		createdAtKpi.OldValue.Should().BeEmpty();
		createdAtKpi.NewValue.Replace("-","/").Should().Be(concern.CreatedAt.ToShortDateString());
		
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
	public void UpdateConcern_CreatesKpiEntries()
	{
		// arrange
		var start = DateTime.Now;
		
		// act
		var concern = CreateConcernInDb();

		// assert
		concern.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(concern.Id, start);

		var createdAtKpi = results.Single(r => r.DataItemChanged == "CreatedAt");
		createdAtKpi.CaseId.Should().Be(concern.CaseId);
		createdAtKpi.DateTimeOfChange.Should().Be(concern.UpdatedAt);
		createdAtKpi.DataItemChanged.Should().Be("CreatedAt");
		createdAtKpi.Operation.Should().Be("Create");
		createdAtKpi.OldValue.Should().BeEmpty();
		createdAtKpi.NewValue.Replace("-","/").Should().Be(concern.CreatedAt.ToShortDateString());
		
		var riskKpi = results.Single(r => r.DataItemChanged == "Risk");
		riskKpi.CaseId.Should().Be(concern.CaseId);
		riskKpi.DateTimeOfChange.Should().Be(concern.UpdatedAt);
		riskKpi.DataItemChanged.Should().Be("Risk");
		riskKpi.Operation.Should().Be("Update");
		riskKpi.OldValue.Should().BeEmpty();
		riskKpi.NewValue.Should().Be(concern.ConcernsRating.Name);

		results.Count.Should().Be(2);
	}

	private ConcernsCase CreateOpenCaseInDb()
	{
		using var context = CreateContext();
		
		var statusId = context.ConcernsStatus.First().Id;
		var ratingId = context.ConcernsRatings.First().Id;

		var concernsCase = BuildOpenCase(statusId, ratingId);

		// act
		context.ConcernsCase.Add(concernsCase);
		context.SaveChanges();

		return concernsCase;
	}
	
	private ConcernsCase BuildOpenCase(int statusId, int ratingId)
	=> new ConcernsCase
        {
        	CreatedAt = _randomGenerator.DateTime(),
        	UpdatedAt = _randomGenerator.DateTime(),
            ClosedAt = _randomGenerator.DateTime(),
        	CreatedBy = _randomGenerator.NextString(3, 10),
        	TrustUkprn = _randomGenerator.NextString(3, 10),
        	Issue = _randomGenerator.NextString(3, 200),
        	DirectionOfTravel = _randomGenerator.NextString(3, 10),
        	StatusId = statusId,
        	RatingId = ratingId
        };

	private static ConcernKpi GetConcernKpi(IDataRecord record)
		=> new (record.GetInt32(0), record.GetDateTime(1), record.GetString(2), record.GetString(3), record.GetString(4), record.GetString(5));

	private List<ConcernKpi> GetKpiResults(int urn, DateTime startTimestamp)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = "SELECT CaseId, DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [kpi].[Concern] WHERE RecordId = @Id AND [Timestamp] >= @StartTimestamp";
		command.Parameters.Add(new SqlParameter("Id", urn));
		command.Parameters.Add(new SqlParameter("StartTimestamp", startTimestamp));
		
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		var kpis = new List<ConcernKpi>();
		while (result.Read())
		{
			kpis.Add(GetConcernKpi(result));
		}

		return kpis;
	}

	private ConcernsRecord CreateConcernInDb()
	{
		var parentCase = CreateOpenCaseInDb();
		
		using var context = CreateContext();
		var statusId = context.ConcernsStatus.First().Id;
		var ratingId = context.ConcernsRatings.First().Id;
		var typeId = context.ConcernsTypes.First().Id;
		var concern = _testDataFactory.BuildOpenConcern(parentCase.Id, statusId, ratingId, typeId);

		context.ConcernsRecord.Add(concern);
		context.SaveChanges();
		
		return concern;
	}
	
	private record ConcernKpi(int CaseId, DateTime DateTimeOfChange, string DataItemChanged, string Operation, string OldValue, string NewValue);
}
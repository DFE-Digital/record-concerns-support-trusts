using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConcernsCaseWork.Data.Tests;

[TestFixture]
public class CaseKpiTests : DatabaseTestFixture
{
	private readonly RandomGenerator _randomGenerator = new ();
	private readonly TestDataFactory _testDataFactory = new ();

	[Test]
	public void CreateNewCase_CreatesKpiEntries()
	{
		// arrange
		var start = DateTime.Now;
		using var context = CreateContext();
		
		var statusId = context.ConcernsStatus.First().Id;
		var ratingId = context.ConcernsRatings.First().Id;
		
		var createdCase = _testDataFactory.BuildOpenCase(statusId, ratingId);

		// act
		context.ConcernsCase.Add(createdCase);
		context.SaveChanges();

		// assert
		createdCase.Urn.Should().BeGreaterThan(0);
		var urn = createdCase.Urn;

		var results = GetKpiResults(urn, start);

		var createdAtKpi = results.Single(r => r.DataItemChanged == "CreatedAt");
		createdAtKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		createdAtKpi.DataItemChanged.Should().Be("CreatedAt");
		createdAtKpi.Operation.Should().Be("Create");
		createdAtKpi.OldValue.Should().BeEmpty();
		createdAtKpi.NewValue.Replace("-","/").Should().Be(createdCase.CreatedAt.ToShortDateString());

		var riskToTrustKpi = results.Single(r => r.DataItemChanged == "Risk to Trust");
		riskToTrustKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		riskToTrustKpi.DataItemChanged.Should().Be("Risk to Trust");
		riskToTrustKpi.Operation.Should().Be("Update");
		riskToTrustKpi.OldValue.Should().BeEmpty();
		riskToTrustKpi.NewValue.Should().Be(createdCase.Rating.Name);

		var directionOfTravelKpi = results.Single(r => r.DataItemChanged == "Direction of Travel");
		directionOfTravelKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		directionOfTravelKpi.DataItemChanged.Should().Be("Direction of Travel");
		directionOfTravelKpi.Operation.Should().Be("Update");
		directionOfTravelKpi.OldValue.Should().BeEmpty();
		directionOfTravelKpi.NewValue.Should().Be(createdCase.DirectionOfTravel);

		results.Count.Should().Be(3);
	}

	[Test]
	public void UpdateCase_RiskToTrust_CreatesKpiEntries()
	{
		// arrange
		var createdCase = CreateOpenCaseInDb();
		var whenCaseCreated = DateTime.Now;

		var originalRating = createdCase.Rating;
		var urn = createdCase.Urn;

		using var context = CreateContext();

		var rating = context.ConcernsRatings.OrderBy(r => r.Id).Last();

		createdCase.UpdatedAt = _randomGenerator.DateTime();
		createdCase.Rating = rating;

		// act
		context.ConcernsCase.Update(createdCase);
		context.SaveChanges();

		// assert
		var results = GetKpiResults(urn, whenCaseCreated);

		var riskToTrustKpi = results.First(r => r.DataItemChanged == "Risk to Trust");
		riskToTrustKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		riskToTrustKpi.DataItemChanged.Should().Be("Risk to Trust");
		riskToTrustKpi.Operation.Should().Be("Update");
		riskToTrustKpi.OldValue.Should().Be(originalRating.Name);
		riskToTrustKpi.NewValue.Should().Be(createdCase.Rating.Name);

		results.Count.Should().Be(1);
	}

	[Test]
	public void UpdateCase_DirectionOfTravel_CreatesKpiEntries()
	{
		// arrange
		var createdCase = CreateOpenCaseInDb();
		var whenCaseCreated = DateTime.Now;
		var originalDirectionOfTravel = createdCase.DirectionOfTravel;
		var urn = createdCase.Urn;
		
		using var context = CreateContext();
		createdCase.UpdatedAt = _randomGenerator.DateTime();
		createdCase.DirectionOfTravel = _randomGenerator.NextString(3, 20);

		// act
		context.ConcernsCase.Update(createdCase);
		context.SaveChanges();
		
		// assert
		var results = GetKpiResults(urn, whenCaseCreated);

		var riskToTrustKpi = results.First(r => r.DataItemChanged == "Direction of Travel");
		riskToTrustKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		riskToTrustKpi.DataItemChanged.Should().Be("Direction of Travel");
		riskToTrustKpi.Operation.Should().Be("Update");
		riskToTrustKpi.OldValue.Should().Be(originalDirectionOfTravel);
		riskToTrustKpi.NewValue.Should().Be(createdCase.DirectionOfTravel);

		results.Count.Should().Be(1);
	}
	
	[Test]
	public void CloseCase_CreatesKpiEntryForClosedAt()
	{
		// arrange
		var createdCase = CreateOpenCaseInDb();
		var whenCaseCreated = DateTime.Now;		
		var urn = createdCase.Urn;
		
		using var context = CreateContext();	
		createdCase.UpdatedAt = _randomGenerator.DateTime();
		createdCase.ClosedAt = _randomGenerator.DateTime();
		
		// act
		context.ConcernsCase.Update(createdCase);
		context.SaveChanges();

		// assert
		var results = GetKpiResults(urn, whenCaseCreated);

		var riskToTrustKpi = results.First(r => r.DataItemChanged == "ClosedAt");
		riskToTrustKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		riskToTrustKpi.DataItemChanged.Should().Be("ClosedAt");
		riskToTrustKpi.Operation.Should().Be("Close");
		riskToTrustKpi.OldValue.Should().BeEmpty();
		riskToTrustKpi.NewValue.Replace("-","/").Should().Be(createdCase.ClosedAt.ToShortDateString());
		
		results.Count.Should().Be(1);
	}
	
	private ConcernsCase CreateOpenCaseInDb()
	{
		using var context = CreateContext();
		
		var statusId = context.ConcernsStatus.First().Id;
		var ratingId = context.ConcernsRatings.First().Id;

		var createdCase = _testDataFactory.BuildOpenCase(statusId, ratingId);

		// act
		context.ConcernsCase.Add(createdCase);
		context.SaveChanges();

		return createdCase;
	}

	private CaseKpi GetCaseKpi(IDataRecord record)
		=> new (record.GetDateTime(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4));

	private List<CaseKpi> GetKpiResults(int urn, DateTime startTimestamp)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = "SELECT DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [kpi].[Case] WHERE CaseId = @Id AND [Timestamp] >= @StartTimestamp";
		command.Parameters.Add(new SqlParameter("Id", urn));
		command.Parameters.Add(new SqlParameter("StartTimestamp", startTimestamp));
		
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		var kpis = new List<CaseKpi>();
		while (result.Read())
		{
			kpis.Add(GetCaseKpi(result));
		}

		return kpis;
	}
	
	private record CaseKpi(DateTime DateTimeOfChange, string DataItemChanged, string Operation, string OldValue, string NewValue);
}
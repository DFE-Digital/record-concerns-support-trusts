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

	[Test]
	public void CreateNewCase_CreatesKpiEntries()
	{
		// act
		var createdCase = TestDbGateway.GenerateTestOpenCaseInDb();
		
		// assert
		createdCase.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(createdCase.Id, 0);

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
		var createdCase = TestDbGateway.GenerateTestOpenCaseInDb();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(createdCase.Id);

		var originalRating = createdCase.Rating;

		var rating = TestDbGateway.GetDifferentRating(originalRating.Id);

		createdCase.UpdatedAt = _randomGenerator.DateTime();
		createdCase.Rating = rating;

		// act
		TestDbGateway.UpdateCase(createdCase);

		// assert
		var results = GetKpiResults(createdCase.Id, maxKpiIdAfterCreate);

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
		var createdCase = TestDbGateway.GenerateTestOpenCaseInDb();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(createdCase.Id);
		var originalDirectionOfTravel = createdCase.DirectionOfTravel;
		
		using var context = CreateContext();
		createdCase.UpdatedAt = _randomGenerator.DateTime();
		createdCase.DirectionOfTravel = _randomGenerator.NextString(3, 20);

		// act
		context.ConcernsCase.Update(createdCase);
		context.SaveChanges();
		
		// assert
		var results = GetKpiResults(createdCase.Id, maxKpiIdAfterCreate);

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
		var createdCase = TestDbGateway.GenerateTestOpenCaseInDb();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(createdCase.Id);
		
		using var context = CreateContext();	
		createdCase.UpdatedAt = _randomGenerator.DateTime();
		createdCase.ClosedAt = _randomGenerator.DateTime();
		
		// act
		context.ConcernsCase.Update(createdCase);
		context.SaveChanges();

		// assert
		var results = GetKpiResults(createdCase.Id, maxKpiIdAfterCreate);

		var riskToTrustKpi = results.First(r => r.DataItemChanged == "ClosedAt");
		riskToTrustKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		riskToTrustKpi.DataItemChanged.Should().Be("ClosedAt");
		riskToTrustKpi.Operation.Should().Be("Close");
		riskToTrustKpi.OldValue.Should().BeEmpty();
		riskToTrustKpi.NewValue.Replace("-","/").Should().Be(createdCase.ClosedAt.ToShortDateString());
		
		results.Count.Should().Be(1);
	}
	
	private List<CaseKpi> GetKpiResults(int caseId, int previousMaxKpiId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = "SELECT DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [kpi].[Case] WHERE CaseId = @Id AND Id > @PreviousMaxKpiId";
		command.Parameters.Add(new SqlParameter("Id", caseId));
		command.Parameters.Add(new SqlParameter("PreviousMaxKpiId", previousMaxKpiId));
		
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		var kpis = new List<CaseKpi>();
		while (result.Read())
		{
			kpis.Add(BuildCaseKpi(result));
		}

		return kpis;
	}
	
	private int GetMaxKpiIdForCase(int caseId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = "SELECT MAX(Id) FROM [kpi].[Case] WHERE CaseId = @Id";
		command.Parameters.Add(new SqlParameter("Id", caseId));
		
		context.Database.OpenConnection();
		var maxKpiId = command.ExecuteScalar() ?? 0;

		return (int)maxKpiId;
	}

	private static CaseKpi BuildCaseKpi(IDataRecord record) => new (record.GetDateTime(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4));
	
	private record CaseKpi(DateTime DateTimeOfChange, string DataItemChanged, string Operation, string OldValue, string NewValue);
}
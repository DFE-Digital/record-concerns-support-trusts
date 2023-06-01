using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Data.Tests.DbGateways;
using ConcernsCaseWork.Data.Tests.Helpers;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConcernsCaseWork.Data.Tests.Tests;

[TestFixture]
public class CaseKpiTests : DatabaseTestFixture
{
	private readonly RandomGenerator _randomGenerator = new ();
	private readonly TestCaseDbGateway _gateway = new ();

	[Test]
	public void CreateNewCase_CreatesKpiEntries()
	{
		// act
		var createdCase = _gateway.GenerateTestOpenCase();
		
		// assert
		createdCase.Id.Should().BeGreaterThan(0);

		var results = GetKpiResults(createdCase.Id, 0);

		var createdAtKpi = results.Single(r => r.DataItemChanged == "CreatedAt");
		createdAtKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		createdAtKpi.Operation.Should().Be("Create");
		createdAtKpi.OldValue.Should().BeEmpty();
		createdAtKpi.NewValue.Should().Be(createdCase.CreatedAt.ToKpiDateFormat());

		var riskToTrustKpi = results.Single(r => r.DataItemChanged == "Risk to Trust");
		riskToTrustKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		riskToTrustKpi.Operation.Should().Be("Update");
		riskToTrustKpi.OldValue.Should().BeEmpty();
		riskToTrustKpi.NewValue.Should().Be(createdCase.Rating.Name);

		var directionOfTravelKpi = results.Single(r => r.DataItemChanged == "Direction of Travel");
		directionOfTravelKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		directionOfTravelKpi.Operation.Should().Be("Update");
		directionOfTravelKpi.OldValue.Should().BeEmpty();
		directionOfTravelKpi.NewValue.Should().Be(createdCase.DirectionOfTravel);
		
		var territoryKpi = results.Single(r => r.DataItemChanged == "Territory");
		territoryKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		territoryKpi.Operation.Should().Be("Update");
		territoryKpi.OldValue.Should().BeEmpty();
		territoryKpi.NewValue.Should().Be(createdCase.Territory.ToString());

		var caseOwnerKpi = results.Single(r => r.DataItemChanged == "Case Owner");
		caseOwnerKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		caseOwnerKpi.Operation.Should().Be("Update");
		caseOwnerKpi.OldValue.Should().BeEmpty();
		caseOwnerKpi.NewValue.Should().Be(createdCase.CreatedBy.ToString());

		results.Count.Should().Be(5);
	}

	[Test]
	public void UpdateCase_RiskToTrust_CreatesKpiEntries()
	{
		// arrange
		var createdCase = _gateway.GenerateTestOpenCase();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(createdCase.Id);

		var originalRating = createdCase.Rating;

		var rating = _gateway.GetDifferentCaseRating(originalRating.Id);

		createdCase.UpdatedAt = _randomGenerator.DateTime();
		createdCase.Rating = rating;

		// act
		_gateway.UpdateCase(createdCase);

		// assert
		var results = GetKpiResults(createdCase.Id, maxKpiIdAfterCreate);

		var riskToTrustKpi = results.First(r => r.DataItemChanged == "Risk to Trust");
		riskToTrustKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		riskToTrustKpi.Operation.Should().Be("Update");
		riskToTrustKpi.OldValue.Should().Be(originalRating.Name);
		riskToTrustKpi.NewValue.Should().Be(createdCase.Rating.Name);

		results.Count.Should().Be(1);
	}

	[Test]
	public void UpdateCase_DirectionOfTravel_CreatesKpiEntries()
	{
		// arrange
		var createdCase = _gateway.GenerateTestOpenCase();
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
		riskToTrustKpi.Operation.Should().Be("Update");
		riskToTrustKpi.OldValue.Should().Be(originalDirectionOfTravel);
		riskToTrustKpi.NewValue.Should().Be(createdCase.DirectionOfTravel);

		results.Count.Should().Be(1);
	}
	
	[Test]
	public void UpdateCase_Territory_CreatesKpiEntries()
	{
		// arrange
		var createdCase = _gateway.GenerateTestOpenCase();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(createdCase.Id);
		var originalTerritory = createdCase.Territory;
		
		using var context = CreateContext();
		createdCase.UpdatedAt = _randomGenerator.DateTime();
		createdCase.Territory = _randomGenerator.Enumeration<Territory>();

		// act
		context.ConcernsCase.Update(createdCase);
		context.SaveChanges();
		
		// assert
		var results = GetKpiResults(createdCase.Id, maxKpiIdAfterCreate);

		var riskToTrustKpi = results.First(r => r.DataItemChanged == "Territory");
		riskToTrustKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		riskToTrustKpi.Operation.Should().Be("Update");
		riskToTrustKpi.OldValue.Should().Be(originalTerritory.ToString());
		riskToTrustKpi.NewValue.Should().Be(createdCase.Territory.ToString());

		results.Count.Should().Be(1);
	}

	[Test]
	public void UpdateCase_CaseOwner_CreatesKpiEntries()
	{
		// arrange
		var createdCase = _gateway.GenerateTestOpenCase();
		var maxKpiIdAfterCreate = GetMaxKpiIdForCase(createdCase.Id);
		var originalCreatedBy = createdCase.CreatedBy;

		using var context = CreateContext();
		createdCase.UpdatedAt = _randomGenerator.DateTime();
		createdCase.CreatedBy = _randomGenerator.NextString(3, 10);

		// act
		context.ConcernsCase.Update(createdCase);
		context.SaveChanges();

		// assert
		var results = GetKpiResults(createdCase.Id, maxKpiIdAfterCreate);

		var riskToTrustKpi = results.First(r => r.DataItemChanged == "Case Owner");
		riskToTrustKpi.DateTimeOfChange.Should().Be(createdCase.UpdatedAt);
		riskToTrustKpi.Operation.Should().Be("Update");
		riskToTrustKpi.OldValue.Should().Be(originalCreatedBy.ToString());
		riskToTrustKpi.NewValue.Should().Be(createdCase.CreatedBy.ToString());

		results.Count.Should().Be(1);
	}
	
	[Test]
	public void CloseCase_CreatesKpiEntryForClosedAt()
	{
		// arrange
		var createdCase = _gateway.GenerateTestOpenCase();
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
		riskToTrustKpi.Operation.Should().Be("Close");
		riskToTrustKpi.OldValue.Should().BeEmpty();
		riskToTrustKpi.NewValue.Should().Be(createdCase.ClosedAt.Value.ToKpiDateFormat());
		
		results.Count.Should().Be(1);
	}
	
	private List<CaseKpi> GetKpiResults(int caseId, int previousMaxKpiId)
	{
		// Trigger might not have executed by the time we get the data
		// Not sure why so lets try a polling approach to wait for the trigger to finish
		// The docs say triggers always execute during insert, but that isn't the behaviour we see all the time
		var result = new List<CaseKpi>();

		for (var idx = 0; idx < 10; idx++)
		{
			result = QueryKpiResults(caseId, previousMaxKpiId);

			if (result.Count == 0)
			{
				Thread.Sleep(100);
			}
			else 
			{
				break;
			}
		}

		return result;
	}

	private List<CaseKpi> QueryKpiResults(int caseId, int previousMaxKpiId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();

		command.CommandText = "SELECT DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [concerns].[kpi-Case] WHERE CaseId = @Id AND Id > @PreviousMaxKpiId";
		command.Parameters.Add(new SqlParameter("Id", caseId));
		command.Parameters.Add(new SqlParameter("PreviousMaxKpiId", previousMaxKpiId));

		context.Database.OpenConnection();
		using var result = command.ExecuteReader();

		var kpiResults = new List<CaseKpi>();
		while (result.Read())
		{
			kpiResults.Add(BuildCaseKpi(result));
		}

		return kpiResults;
	}
	
	private int GetMaxKpiIdForCase(int caseId)
	{
		using var context = CreateContext();
		using var command = context.Database.GetDbConnection().CreateCommand();
		
		command.CommandText = "SELECT MAX(Id) FROM [concerns].[kpi-Case] WHERE CaseId = @Id";
		command.Parameters.Add(new SqlParameter("Id", caseId));
		
		context.Database.OpenConnection();
		var maxKpiId = command.ExecuteScalar() ?? 0;

		return (int)maxKpiId;
	}

	private static CaseKpi BuildCaseKpi(IDataRecord record) => new (record.GetDateTime(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4));
	
	private record CaseKpi(DateTime DateTimeOfChange, string DataItemChanged, string Operation, string OldValue, string NewValue);
}
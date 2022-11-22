using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests;

[TestFixture]
public class ConcernsCaseTriggerTests : DatabaseTestFixture
{
	private readonly RandomGenerator _randomGenerator = new ();
	
	[Test]
	public void CreateNewConcernsCase_CreatesKpiEntry()
	{
		// arrange
		using var context = CreateContext();
		
		// act
		var concernsCase = CreateOpenConcernsCaseInDb();
		
		// assert
		concernsCase.Urn.Should().BeGreaterThan(0);
		var urn = concernsCase.Urn;
		
		using var command = context.Database.GetDbConnection().CreateCommand();
		command.CommandText = "SELECT COUNT(*) FROM [kpi].[Case] WHERE CaseId = @Id";
		command.Parameters.Add(new SqlParameter("Id", urn));
		context.Database.OpenConnection();
		var result = command.ExecuteScalar();
		
		result.Should().Be(3);
	}
	
	[Test]
	public void CreateNewConcernsCase_CreatesKpiEntries()
	{
		// arrange
		using var context = CreateContext();
		
		var statusId = context.ConcernsStatus.First().Id;
		var ratingId = context.ConcernsRatings.First().Id;
		
		var concernsCase = BuildOpenConcernsCase(statusId, ratingId);

		// act
		context.ConcernsCase.Add(concernsCase);
		context.SaveChanges();

		// assert
		concernsCase.Urn.Should().BeGreaterThan(0);
		var urn = concernsCase.Urn;
		
		using var command = context.Database.GetDbConnection().CreateCommand();
		command.CommandText = "SELECT DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [kpi].[Case] WHERE CaseId = @Id";
		command.Parameters.Add(new SqlParameter("Id", urn));
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		result.Read();
		result.GetDateTime(0).Should().Be(concernsCase.UpdatedAt); // DateTimeOfChange
		result.GetString(1).Should().Be("CreatedAt"); // DataItemChanged
		result.GetString(2).Should().Be("Create"); // Operation
		result.GetString(3).Should().BeEmpty(); // OldValue
		result.GetString(4).Replace("-","/").Should().Be(concernsCase.CreatedAt.ToShortDateString()); // NewValue

		result.Read();
		result.GetDateTime(0).Should().Be(concernsCase.UpdatedAt); // DateTimeOfChange
		result.GetString(1).Should().Be("Risk to Trust"); // DataItemChanged
		result.GetString(2).Should().Be("Update"); // Operation
		result.GetString(3).Should().BeEmpty(); // OldValue
		result.GetString(4).Should().Be(concernsCase.Rating.Name); // NewValue
		
		result.Read();
		result.GetDateTime(0).Should().Be(concernsCase.UpdatedAt); // DateTimeOfChange
		result.GetString(1).Should().Be("Direction of Travel"); // DataItemChanged
		result.GetString(2).Should().Be("Update"); // Operation
		result.GetString(3).Should().BeEmpty(); // OldValue
		result.GetString(4).Should().Be(concernsCase.DirectionOfTravel); // NewValue
	}
	
	[Test]
	public void UpdateConcernsCase_RiskToTrust_CreatesKpiEntries()
	{
		// arrange
		var concernsCase = CreateOpenConcernsCaseInDb();
		var originalRating = concernsCase.Rating;
		var urn = concernsCase.Urn;
		
		// act
		using var context = CreateContext();
		
		var rating = context.ConcernsRatings.OrderBy(r => r.Id).Last();

		concernsCase.UpdatedAt = _randomGenerator.DateTime();
		concernsCase.Rating = rating;

		// act
		var now = DateTime.Now;
		context.ConcernsCase.Update(concernsCase);
		context.SaveChanges();

		// assert
		using var command = context.Database.GetDbConnection().CreateCommand();
		command.CommandText = "SELECT DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [kpi].[Case] WHERE CaseId = @Id AND [Timestamp] >= @Now";
		command.Parameters.Add(new SqlParameter("Id", urn));
		command.Parameters.Add(new SqlParameter("Now", now));
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		result.Read();
		result.GetDateTime(0).Should().Be(concernsCase.UpdatedAt); // DateTimeOfChange
		result.GetString(1).Should().Be("Risk to Trust"); // DataItemChanged
		result.GetString(2).Should().Be("Update"); // Operation
		result.GetString(3).Should().Be(originalRating.Name); // OldValue
		result.GetString(4).Should().Be(rating.Name); // NewValue
	}
	
	[Test]
	public void UpdateConcernsCase_DirectionOfTravel_CreatesKpiEntries()
	{
		// arrange
		var concernsCase = CreateOpenConcernsCaseInDb();
		var originalRating = concernsCase.Rating;
		var originalDirectionOfTravel = concernsCase.DirectionOfTravel;
		var urn = concernsCase.Urn;
		
		// act
		using var context = CreateContext();
		
		var status = context.ConcernsStatus.OrderBy(s => s.Id).Last();
		var rating = context.ConcernsRatings.OrderBy(r => r.Id).Last();

		concernsCase.UpdatedAt = _randomGenerator.DateTime();
		concernsCase.DirectionOfTravel = _randomGenerator.NextString(3, 20);

		// act
		var now = DateTime.Now;
		context.ConcernsCase.Update(concernsCase);
		context.SaveChanges();

		// assert
		using var command = context.Database.GetDbConnection().CreateCommand();
		command.CommandText = "SELECT DateTimeOfChange, DataItemChanged, Operation, ISNULL(OldValue,''), NewValue FROM [kpi].[Case] WHERE CaseId = @Id AND [Timestamp] >= @Now";
		command.Parameters.Add(new SqlParameter("Id", urn));
		command.Parameters.Add(new SqlParameter("Now", now));
		context.Database.OpenConnection();
		using var result = command.ExecuteReader();
		
		result.Read();
		result.GetDateTime(0).Should().Be(concernsCase.UpdatedAt); // DateTimeOfChange
		result.GetString(1).Should().Be("Direction of Travel"); // DataItemChanged
		result.GetString(2).Should().Be("Update"); // Operation
		result.GetString(3).Should().Be(originalDirectionOfTravel); // OldValue
		result.GetString(4).Should().Be(concernsCase.DirectionOfTravel); // NewValue
	}

	private ConcernsCase CreateOpenConcernsCaseInDb()
	{
		using var context = CreateContext();
		
		var statusId = context.ConcernsStatus.First().Id;
		var ratingId = context.ConcernsRatings.First().Id;

		var concernsCase = BuildOpenConcernsCase(statusId, ratingId);

		// act
		context.ConcernsCase.Add(concernsCase);
		context.SaveChanges();

		return concernsCase;
	}

	private ConcernsCase BuildOpenConcernsCase(int statusId, int ratingId)
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
}
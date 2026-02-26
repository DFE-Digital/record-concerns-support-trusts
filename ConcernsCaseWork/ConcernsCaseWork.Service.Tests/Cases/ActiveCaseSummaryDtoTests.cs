using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Ratings;

namespace ConcernsCaseWork.Service.Tests.Cases
{
	public class ActiveCaseSummaryDtoTests
    {
		[Test]
		public void Can_Create_ActiveCaseSummaryDto_With_ActiveConcerns()
		{
			// Arrange
			var rating = new RatingDto("Red", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, 1);
			var concern = new CaseSummaryDto.ConcernSummaryDto("Test Concern", rating, DateTime.UtcNow);
			var activeConcerns = new List<CaseSummaryDto.ConcernSummaryDto> { concern };

			var dto = new ActiveCaseSummaryDto
			{
				CaseUrn = 123,
				CreatedBy = "user",
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				StatusName = "Open",
				Rating = rating,
				TrustUkPrn = "UKPRN123",
				Decisions = new List<CaseSummaryDto.ActionDecisionSummaryDto>(),
				FinancialPlanCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>(),
				NoticesToImprove = new List<CaseSummaryDto.ActionDecisionSummaryDto>(),
				NtiWarningLetters = new List<CaseSummaryDto.ActionDecisionSummaryDto>(),
				NtisUnderConsideration = new List<CaseSummaryDto.ActionDecisionSummaryDto>(),
				SrmaCases = new List<CaseSummaryDto.ActionDecisionSummaryDto>(),
				TrustFinancialForecasts = new List<CaseSummaryDto.ActionDecisionSummaryDto>(),
				TargetedTrustEngagements = new List<CaseSummaryDto.ActionDecisionSummaryDto>(),
				Division = Division.SFSO,
				Area = "Area 1",
				TeamLedBy = "Team Lead",
				ActiveConcerns = activeConcerns
			};

			// Act & Assert
			Assert.Multiple(() =>
			{
				Assert.That(dto.CaseUrn, Is.EqualTo(123));
				Assert.That(dto.CreatedBy, Is.EqualTo("user"));
				Assert.That(dto.StatusName, Is.EqualTo("Open"));
				Assert.That(dto.Rating, Is.EqualTo(rating));
				Assert.That(dto.TrustUkPrn, Is.EqualTo("UKPRN123"));
				Assert.That(dto.ActiveConcerns, Is.Not.Null);
				Assert.That(dto.ActiveConcerns.Count(), Is.EqualTo(1));
				Assert.That(dto.ActiveConcerns.First().Name, Is.EqualTo("Test Concern"));
			});
		}
	}
}

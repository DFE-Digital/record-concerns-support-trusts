using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using Service.TRAMS.RecordWhistleblower;
using System.Text.Json;

namespace Service.TRAMS.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseDtoTests
	{
		[Test]
		public void WhenCaseDtoSerializeAndDeserialize_IsSuccessful()
		{
			// arrange
			var caseDto = CaseDtoFactory.CreateCaseDto();
			var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
			var caseStrDto = JsonSerializer.Serialize(caseDto, options);
			var expectedCaseDto = JsonSerializer.Deserialize<CaseDto>(caseStrDto, options);
			
			// assert
			Assert.That(caseDto, Is.Not.Null);
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(caseDto.Description, Is.EqualTo(expectedCaseDto.Description));
			Assert.That(caseDto.Issue, Is.EqualTo(expectedCaseDto.Issue));
			Assert.That(caseDto.Status, Is.EqualTo(expectedCaseDto.Status));
			Assert.That(caseDto.Urn, Is.EqualTo(expectedCaseDto.Urn));
			Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCaseDto.ClosedAt));
			Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCaseDto.CreatedAt));
			Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCaseDto.CreatedBy));
			Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCaseDto.CrmEnquiry));
			Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCaseDto.CurrentStatus));
			Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCaseDto.DeEscalation));
			Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCaseDto.NextSteps));
			Assert.That(caseDto.ResolutionStrategy, Is.EqualTo(expectedCaseDto.ResolutionStrategy));
			Assert.That(caseDto.ReviewAt, Is.EqualTo(expectedCaseDto.ReviewAt));
			Assert.That(caseDto.UpdateAt, Is.EqualTo(expectedCaseDto.UpdateAt));
			Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCaseDto.DirectionOfTravel));
			Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCaseDto.ReasonAtReview));
			Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCaseDto.TrustUkPrn));
		}
	}
}
using ConcernsCaseWork.Shared.Tests.Factory;
using Newtonsoft.Json;
using NUnit.Framework;
using Service.TRAMS.Cases;

namespace Service.TRAMS.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseDtoTests
	{
		[Test]
		public void WhenCaseDtoSerializeAndDeserialize_IsSuccessful()
		{
			// arrange
			var caseDto = CaseDtoFactory.BuildCaseDto();
			var caseStrDto = JsonConvert.SerializeObject(caseDto);
			var expectedCaseDto = JsonConvert.DeserializeObject<CaseDto>(caseStrDto);
			
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
			Assert.That(caseDto.UpdatedAt, Is.EqualTo(expectedCaseDto.UpdatedAt));
			Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCaseDto.DirectionOfTravel));
			Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCaseDto.ReasonAtReview));
			Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCaseDto.TrustUkPrn));
		}
	}
}
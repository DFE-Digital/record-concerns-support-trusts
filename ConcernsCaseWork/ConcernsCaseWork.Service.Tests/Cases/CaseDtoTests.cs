using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseDtoTests
	{
		[Test]
		public void WhenCaseDtoSerializeAndDeserialize_IsSuccessful()
		{
			// arrange
			var caseDto = CaseFactory.BuildCaseDto();
			var caseStrDto = JsonConvert.SerializeObject(caseDto);
			var expectedCaseDto = JsonConvert.DeserializeObject<CaseDto>(caseStrDto);
			
			// assert
			Assert.That(caseDto, Is.Not.Null);
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(caseDto.Description, Is.EqualTo(expectedCaseDto.Description));
			Assert.That(caseDto.Issue, Is.EqualTo(expectedCaseDto.Issue));
			Assert.That(caseDto.StatusId, Is.EqualTo(expectedCaseDto.StatusId));
			Assert.That(caseDto.Urn, Is.EqualTo(expectedCaseDto.Urn));
			Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCaseDto.ClosedAt));
			Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCaseDto.CreatedAt));
			Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCaseDto.CreatedBy));
			Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCaseDto.CrmEnquiry));
			Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCaseDto.CurrentStatus));
			Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCaseDto.DeEscalation));
			Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCaseDto.NextSteps));
			Assert.That(caseDto.CaseAim, Is.EqualTo(expectedCaseDto.CaseAim));
			Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(expectedCaseDto.DeEscalationPoint));
			Assert.That(caseDto.ReviewAt, Is.EqualTo(expectedCaseDto.ReviewAt));
			Assert.That(caseDto.UpdatedAt, Is.EqualTo(expectedCaseDto.UpdatedAt));
			Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCaseDto.DirectionOfTravel));
			Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCaseDto.ReasonAtReview));
			Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCaseDto.TrustUkPrn));
		}
		
		[Test]
		public void WhenCreateCaseDtoSerializeAndDeserialize_ToCaseDto_IsSuccessful()
		{
			// arrange
			var createCaseDto = CaseFactory.BuildCreateCaseDto();
			var createCaseStrDto = JsonConvert.SerializeObject(createCaseDto);
			var expectedCaseDto = JsonConvert.DeserializeObject<CaseDto>(createCaseStrDto);
			
			// assert
			Assert.That(createCaseDto, Is.Not.Null);
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(expectedCaseDto.Urn, Is.EqualTo(0));
			Assert.That(createCaseDto.Issue, Is.EqualTo(expectedCaseDto.Issue));
			Assert.That(createCaseDto.StatusId, Is.EqualTo(expectedCaseDto.StatusId));
			Assert.That(createCaseDto.ClosedAt, Is.EqualTo(expectedCaseDto.ClosedAt));
			Assert.That(createCaseDto.CreatedAt, Is.EqualTo(expectedCaseDto.CreatedAt));
			Assert.That(createCaseDto.CreatedBy, Is.EqualTo(expectedCaseDto.CreatedBy));
			Assert.That(createCaseDto.CrmEnquiry, Is.EqualTo(expectedCaseDto.CrmEnquiry));
			Assert.That(createCaseDto.CurrentStatus, Is.EqualTo(expectedCaseDto.CurrentStatus));
			Assert.That(createCaseDto.DeEscalation, Is.EqualTo(expectedCaseDto.DeEscalation));
			Assert.That(createCaseDto.NextSteps, Is.EqualTo(expectedCaseDto.NextSteps));
			Assert.That(createCaseDto.CaseAim, Is.EqualTo(expectedCaseDto.CaseAim));
			Assert.That(createCaseDto.DeEscalationPoint, Is.EqualTo(expectedCaseDto.DeEscalationPoint));
			Assert.That(createCaseDto.ReviewAt, Is.EqualTo(expectedCaseDto.ReviewAt));
			Assert.That(createCaseDto.UpdatedAt, Is.EqualTo(expectedCaseDto.UpdatedAt));
			Assert.That(createCaseDto.DirectionOfTravel, Is.EqualTo(expectedCaseDto.DirectionOfTravel));
			Assert.That(createCaseDto.ReasonAtReview, Is.EqualTo(expectedCaseDto.ReasonAtReview));
			Assert.That(createCaseDto.TrustUkPrn, Is.EqualTo(expectedCaseDto.TrustUkPrn));
		}
	}
}
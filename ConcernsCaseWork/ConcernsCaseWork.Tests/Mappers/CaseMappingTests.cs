using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using Service.TRAMS.Status;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class CaseMappingTests
	{
		[Test]
		public void WhenMapCreateCaseModel_Returns_CreateCaseDto()
		{
			// arrange
			var createCaseModel = CaseFactory.BuildCreateCaseModel();

			// act
			var createCaseDto = CaseMapping.Map(createCaseModel);

			// assert
			Assert.That(createCaseDto, Is.Not.Null);
			Assert.That(createCaseDto.Description, Is.EqualTo(createCaseModel.Description));
			Assert.That(createCaseDto.Issue, Is.EqualTo(createCaseModel.Issue));
			Assert.That(createCaseDto.Status, Is.EqualTo(createCaseModel.Status));
			Assert.That(createCaseDto.Urn, Is.EqualTo(createCaseModel.Urn));
			Assert.That(createCaseDto.ClosedAt, Is.EqualTo(createCaseModel.ClosedAt));
			Assert.That(createCaseDto.CreatedAt, Is.EqualTo(createCaseModel.CreatedAt));
			Assert.That(createCaseDto.CreatedBy, Is.EqualTo(createCaseModel.CreatedBy));
			Assert.That(createCaseDto.CrmEnquiry, Is.EqualTo(createCaseModel.CrmEnquiry));
			Assert.That(createCaseDto.CurrentStatus, Is.EqualTo(createCaseModel.CurrentStatus));
			Assert.That(createCaseDto.DeEscalation, Is.EqualTo(createCaseModel.DeEscalation));
			Assert.That(createCaseDto.NextSteps, Is.EqualTo(createCaseModel.NextSteps));
			Assert.That(createCaseDto.ResolutionStrategy, Is.EqualTo(createCaseModel.ResolutionStrategy));
			Assert.That(createCaseDto.ReviewAt, Is.EqualTo(createCaseModel.ReviewAt));
			Assert.That(createCaseDto.UpdatedAt, Is.EqualTo(createCaseModel.UpdatedAt));
			Assert.That(createCaseDto.DirectionOfTravel, Is.EqualTo(createCaseModel.DirectionOfTravel));
			Assert.That(createCaseDto.ReasonAtReview, Is.EqualTo(createCaseModel.ReasonAtReview));
			Assert.That(createCaseDto.TrustUkPrn, Is.EqualTo(createCaseModel.TrustUkPrn));
		}
		
		[Test]
		public void WhenMapCaseDto_Returns_CaseModel()
		{
			// arrange
			var caseDto = CaseFactory.BuildCaseDto();

			// act
			var caseModel = CaseMapping.Map(caseDto, Status.Close.ToString());

			// assert
			Assert.That(caseModel, Is.Not.Null);
			Assert.That(caseModel.Description, Is.EqualTo(caseDto.Description));
			Assert.That(caseModel.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(caseModel.Status, Is.EqualTo(caseDto.Status));
			Assert.That(caseModel.StatusName, Is.EqualTo(Status.Close.ToString()));
			Assert.That(caseModel.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(caseModel.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(caseModel.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(caseModel.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(caseModel.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(caseModel.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(caseModel.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(caseModel.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(caseModel.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(caseModel.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(caseModel.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(caseModel.UpdatedAt, Is.EqualTo(caseDto.UpdatedAt));
			Assert.That(caseModel.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(caseModel.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(caseModel.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}
		
		[Test]
		public void WhenMapClosure_Returns_CaseDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var caseDto = CaseFactory.BuildCaseDto();
			var statusDto = StatusFactory.BuildStatusDto(Status.Monitoring.ToString(), 1);

			// act
			caseDto = CaseMapping.MapClosure(patchCaseModel, caseDto, statusDto);

			// assert
			Assert.That(caseDto, Is.Not.Null);
			Assert.That(caseDto.Description, Is.EqualTo(caseDto.Description));
			Assert.That(caseDto.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(caseDto.Status, Is.EqualTo(caseDto.Status));
			Assert.That(caseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(caseDto.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(caseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(caseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(caseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(caseDto.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(caseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(caseDto.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(caseDto.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(caseDto.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(caseDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
			Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(caseDto.ReasonAtReview, Is.EqualTo(patchCaseModel.ReasonAtReview));
			Assert.That(caseDto.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}

		[Test]
		public void WhenMapDirectionOfTravel_Returns_CaseDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var caseDto = CaseFactory.BuildCaseDto();
			
			// act
			caseDto = CaseMapping.MapDirectionOfTravel(patchCaseModel, caseDto);

			// assert
			Assert.That(caseDto, Is.Not.Null);
			Assert.That(caseDto.Description, Is.EqualTo(caseDto.Description));
			Assert.That(caseDto.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(caseDto.Status, Is.EqualTo(caseDto.Status));
			Assert.That(caseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(caseDto.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(caseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(caseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(caseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(caseDto.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(caseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(caseDto.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(caseDto.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(caseDto.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(caseDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
			Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(patchCaseModel.DirectionOfTravel));
			Assert.That(caseDto.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(caseDto.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}
	}
}
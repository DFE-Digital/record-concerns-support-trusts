using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using Service.TRAMS.Ratings;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using Service.TRAMS.Types;
using System;
using System.Collections.Generic;

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
			Assert.That(createCaseDto.Issue, Is.EqualTo(createCaseModel.Issue));
			Assert.That(createCaseDto.StatusUrn, Is.EqualTo(createCaseModel.StatusUrn));
			Assert.That(createCaseDto.ClosedAt, Is.EqualTo(createCaseModel.ClosedAt));
			Assert.That(createCaseDto.CreatedAt, Is.EqualTo(createCaseModel.CreatedAt));
			Assert.That(createCaseDto.CreatedBy, Is.EqualTo(createCaseModel.CreatedBy));
			Assert.That(createCaseDto.CrmEnquiry, Is.EqualTo(createCaseModel.CrmEnquiry));
			Assert.That(createCaseDto.CurrentStatus, Is.EqualTo(createCaseModel.CurrentStatus));
			Assert.That(createCaseDto.DeEscalation, Is.EqualTo(createCaseModel.DeEscalation));
			Assert.That(createCaseDto.NextSteps, Is.EqualTo(createCaseModel.NextSteps));
			Assert.That(createCaseDto.CaseAim, Is.EqualTo(createCaseModel.CaseAim));
			Assert.That(createCaseDto.DeEscalationPoint, Is.EqualTo(createCaseModel.DeEscalationPoint));
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
			var caseModel = CaseMapping.Map(caseDto, StatusEnum.Close.ToString());

			// assert
			Assert.That(caseModel, Is.Not.Null);
			Assert.That(caseModel.Description, Is.EqualTo(caseDto.Description));
			Assert.That(caseModel.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(caseModel.StatusUrn, Is.EqualTo(caseDto.StatusUrn));
			Assert.That(caseModel.StatusName, Is.EqualTo(StatusEnum.Close.ToString()));
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
			var statusDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 1);

			// act
			var expectedCaseDto = CaseMapping.MapClosure(patchCaseModel, caseDto, statusDto);

			// assert
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(expectedCaseDto.Description, Is.EqualTo(caseDto.Description));
			Assert.That(expectedCaseDto.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(expectedCaseDto.StatusUrn, Is.EqualTo(statusDto.Urn));
			Assert.That(expectedCaseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(expectedCaseDto.ClosedAt, Is.Not.Null);
			Assert.That(expectedCaseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(expectedCaseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(expectedCaseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(expectedCaseDto.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(expectedCaseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(expectedCaseDto.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(expectedCaseDto.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(expectedCaseDto.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(expectedCaseDto.ReviewAt, Is.Not.Null);
			Assert.That(expectedCaseDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
			Assert.That(expectedCaseDto.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(expectedCaseDto.ReasonAtReview, Is.EqualTo(patchCaseModel.ReasonAtReview));
			Assert.That(expectedCaseDto.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}

		[Test]
		public void WhenMapClosure_When_ClosedAt_ReviewAt_IsNotNull_Returns_CaseDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var currentDate = DateTimeOffset.Now;
			patchCaseModel.ClosedAt = currentDate;
			patchCaseModel.ReviewAt = currentDate;
			var caseDto = CaseFactory.BuildCaseDto();
			var statusDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 1);

			// act
			var expectedCaseDto = CaseMapping.MapClosure(patchCaseModel, caseDto, statusDto);

			// assert
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(expectedCaseDto.Description, Is.EqualTo(caseDto.Description));
			Assert.That(expectedCaseDto.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(expectedCaseDto.StatusUrn, Is.EqualTo(statusDto.Urn));
			Assert.That(expectedCaseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(expectedCaseDto.ClosedAt, Is.EqualTo(patchCaseModel.ClosedAt));
			Assert.That(expectedCaseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(expectedCaseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(expectedCaseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(expectedCaseDto.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(expectedCaseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(expectedCaseDto.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(expectedCaseDto.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(expectedCaseDto.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(expectedCaseDto.ReviewAt, Is.EqualTo(patchCaseModel.ReviewAt));
			Assert.That(expectedCaseDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
			Assert.That(expectedCaseDto.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(expectedCaseDto.ReasonAtReview, Is.EqualTo(patchCaseModel.ReasonAtReview));
			Assert.That(expectedCaseDto.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}


		[Test]
		public void WhenMapDirectionOfTravel_Returns_CaseDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var caseDto = CaseFactory.BuildCaseDto();
			
			// act
			var expectedCaseDto = CaseMapping.MapDirectionOfTravel(patchCaseModel, caseDto);

			// assert
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(expectedCaseDto.Description, Is.EqualTo(caseDto.Description));
			Assert.That(expectedCaseDto.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(expectedCaseDto.StatusUrn, Is.EqualTo(caseDto.StatusUrn));
			Assert.That(expectedCaseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(expectedCaseDto.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(expectedCaseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(expectedCaseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(expectedCaseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(expectedCaseDto.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(expectedCaseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(expectedCaseDto.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(expectedCaseDto.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(expectedCaseDto.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(expectedCaseDto.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(expectedCaseDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
			Assert.That(expectedCaseDto.DirectionOfTravel, Is.EqualTo(patchCaseModel.DirectionOfTravel));
			Assert.That(expectedCaseDto.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(expectedCaseDto.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}
		
		[Test]
		public void WhenMapIssue_Returns_CaseDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var caseDto = CaseFactory.BuildCaseDto();
			
			// act
			var expectedCaseDto = CaseMapping.MapIssue(patchCaseModel, caseDto);

			// assert
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(expectedCaseDto.Description, Is.EqualTo(caseDto.Description));
			Assert.That(expectedCaseDto.Issue, Is.EqualTo(patchCaseModel.Issue));
			Assert.That(expectedCaseDto.StatusUrn, Is.EqualTo(caseDto.StatusUrn));
			Assert.That(expectedCaseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(expectedCaseDto.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(expectedCaseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(expectedCaseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(expectedCaseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(expectedCaseDto.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(expectedCaseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(expectedCaseDto.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(expectedCaseDto.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(expectedCaseDto.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(expectedCaseDto.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(expectedCaseDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
			Assert.That(expectedCaseDto.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(expectedCaseDto.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(expectedCaseDto.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}
		
		[Test]
		public void WhenMapCurrentStatus_Returns_CaseDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var caseDto = CaseFactory.BuildCaseDto();
			
			// act
			var expectedCaseDto = CaseMapping.MapCurrentStatus(patchCaseModel, caseDto);

			// assert
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(expectedCaseDto.Description, Is.EqualTo(caseDto.Description));
			Assert.That(expectedCaseDto.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(expectedCaseDto.StatusUrn, Is.EqualTo(caseDto.StatusUrn));
			Assert.That(expectedCaseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(expectedCaseDto.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(expectedCaseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(expectedCaseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(expectedCaseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(expectedCaseDto.CurrentStatus, Is.EqualTo(patchCaseModel.CurrentStatus));
			Assert.That(expectedCaseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(expectedCaseDto.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(expectedCaseDto.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(expectedCaseDto.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(expectedCaseDto.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(expectedCaseDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
			Assert.That(expectedCaseDto.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(expectedCaseDto.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(expectedCaseDto.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}
		
		[Test]
		public void WhenMapCaseAim_Returns_CaseDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var caseDto = CaseFactory.BuildCaseDto();
			
			// act
			var expectedCaseDto = CaseMapping.MapCaseAim(patchCaseModel, caseDto);

			// assert
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(expectedCaseDto.Description, Is.EqualTo(caseDto.Description));
			Assert.That(expectedCaseDto.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(expectedCaseDto.StatusUrn, Is.EqualTo(caseDto.StatusUrn));
			Assert.That(expectedCaseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(expectedCaseDto.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(expectedCaseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(expectedCaseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(expectedCaseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(expectedCaseDto.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(expectedCaseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(expectedCaseDto.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(expectedCaseDto.CaseAim, Is.EqualTo(patchCaseModel.CaseAim));
			Assert.That(expectedCaseDto.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(expectedCaseDto.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(expectedCaseDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
			Assert.That(expectedCaseDto.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(expectedCaseDto.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(expectedCaseDto.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}
		
		[Test]
		public void WhenMapDeEscalationPoint_Returns_CaseDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var caseDto = CaseFactory.BuildCaseDto();
			
			// act
			var expectedCaseDto = CaseMapping.MapDeEscalationPoint(patchCaseModel, caseDto);

			// assert
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(expectedCaseDto.Description, Is.EqualTo(caseDto.Description));
			Assert.That(expectedCaseDto.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(expectedCaseDto.StatusUrn, Is.EqualTo(caseDto.StatusUrn));
			Assert.That(expectedCaseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(expectedCaseDto.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(expectedCaseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(expectedCaseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(expectedCaseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(expectedCaseDto.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(expectedCaseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(expectedCaseDto.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(expectedCaseDto.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(expectedCaseDto.DeEscalationPoint, Is.EqualTo(patchCaseModel.DeEscalationPoint));
			Assert.That(expectedCaseDto.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(expectedCaseDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
			Assert.That(expectedCaseDto.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(expectedCaseDto.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(expectedCaseDto.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}

		[Test]
		public void WhenMapNextSteps_Returns_CaseDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var caseDto = CaseFactory.BuildCaseDto();

			// act
			var expectedCaseDto = CaseMapping.MapNextSteps(patchCaseModel, caseDto);

			// assert
			Assert.That(expectedCaseDto, Is.Not.Null);
			Assert.That(expectedCaseDto.Description, Is.EqualTo(caseDto.Description));
			Assert.That(expectedCaseDto.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(expectedCaseDto.StatusUrn, Is.EqualTo(caseDto.StatusUrn));
			Assert.That(expectedCaseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(expectedCaseDto.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(expectedCaseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(expectedCaseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(expectedCaseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(expectedCaseDto.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(expectedCaseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(expectedCaseDto.NextSteps, Is.EqualTo(patchCaseModel.NextSteps));
			Assert.That(expectedCaseDto.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(expectedCaseDto.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(expectedCaseDto.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(expectedCaseDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
			Assert.That(expectedCaseDto.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(expectedCaseDto.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(expectedCaseDto.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
		}

		[Test]
		public void WhenMapTrustCases_Returns_ListTrustCasesModel()
		{
			// arrange
			var recordsDto = RecordFactory.BuildListRecordDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var casesDto = CaseFactory.BuildListCaseDto();
			var liveStatus = StatusFactory.BuildStatusDto("live", 1);
			var closeStatus = StatusFactory.BuildStatusDto("close", 3);
			var statuses = new List<StatusDto>() { liveStatus, closeStatus };

			// act
			var expectedTrustCasesModel = CaseMapping.MapTrustCases(recordsDto, ratingsDto, typesDto, casesDto, statuses);

			// assert
			Assert.That(expectedTrustCasesModel, Is.Not.Null);
			Assert.That(expectedTrustCasesModel.Count, Is.EqualTo(5));
		}
		
		[Test]
		public void WhenMapTrustCases_MissingType_Returns_ListTrustCasesModel()
		{
			// arrange
			var recordsDto = new List<RecordDto> { RecordFactory.BuildRecordDto(1, 0) };
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = Array.Empty<TypeDto>();
			var casesDto = CaseFactory.BuildListCaseDto();
			var liveStatus = StatusFactory.BuildStatusDto("live", 1);
			var closeStatus = StatusFactory.BuildStatusDto("close", 3);
			var statuses = new List<StatusDto>() { liveStatus, closeStatus };

			// act
			var expectedTrustCasesModel = CaseMapping.MapTrustCases(recordsDto, ratingsDto, typesDto, casesDto, statuses);

			// assert
			Assert.That(expectedTrustCasesModel, Is.Not.Null);
			Assert.That(expectedTrustCasesModel.Count, Is.EqualTo(0));
		}
		
		[Test]
		public void WhenMapTrustCases_MissingRating_Returns_ListTrustCasesModel()
		{
			// arrange
			var recordsDto = new List<RecordDto> { RecordFactory.BuildRecordDto(), RecordFactory.BuildRecordDto(2, 2) };
			var ratingsDto = Array.Empty<RatingDto>();
			var typesDto = TypeFactory.BuildListTypeDto();
			var casesDto = CaseFactory.BuildListCaseDto();
			var liveStatus = StatusFactory.BuildStatusDto("live", 1);
			var closeStatus = StatusFactory.BuildStatusDto("close", 3);
			var statuses = new List<StatusDto>() { liveStatus, closeStatus };

			// act
			var expectedTrustCasesModel = CaseMapping.MapTrustCases(recordsDto, ratingsDto, typesDto, casesDto, statuses);

			// assert
			Assert.That(expectedTrustCasesModel, Is.Not.Null);
			Assert.That(expectedTrustCasesModel.Count, Is.EqualTo(0));
		}
		
		[Test]
		public void WhenMapTrustCases_MissingCaseDto_Returns_ListTrustCasesModel()
		{
			// arrange
			var recordsDto = new List<RecordDto> { RecordFactory.BuildRecordDto(0) };
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var casesDto = CaseFactory.BuildListCaseDto();
			var liveStatus = StatusFactory.BuildStatusDto("live", 1);
			var closeStatus = StatusFactory.BuildStatusDto("close", 3);
			var statuses = new List<StatusDto>() { liveStatus, closeStatus };

			// act
			var expectedTrustCasesModel = CaseMapping.MapTrustCases(recordsDto, ratingsDto, typesDto, casesDto, statuses);

			// assert
			Assert.That(expectedTrustCasesModel, Is.Not.Null);
			Assert.That(expectedTrustCasesModel.Count, Is.EqualTo(0));
		}
	}
}
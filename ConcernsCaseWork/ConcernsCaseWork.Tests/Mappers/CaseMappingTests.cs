﻿using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
using NUnit.Framework;
using System;

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
			Assert.That(createCaseDto.StatusId, Is.EqualTo(createCaseModel.StatusId));
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
			Assert.That(createCaseDto.Division, Is.EqualTo(createCaseModel.Division));
			Assert.That(createCaseDto.Territory, Is.EqualTo(createCaseModel.Territory));
			Assert.That(createCaseDto.Region, Is.EqualTo(createCaseModel.Region));
		}
		
		[TestCase(Division.SFSO, Region.London, Territory.National_Operations, "National Operations")]
		[TestCase(Division.RegionsGroup, Region.London, Territory.National_Operations, "London")]
		public void WhenMapCaseDto_Returns_CaseModel(Division division, Region region, Territory territory, string location)
		{
			// arrange
			var caseDto = CaseFactory.BuildCaseDto();
			caseDto.Urn = 22134234;
			caseDto.Territory = territory;
			caseDto.Region = region;
			caseDto.Division = division;

			// act
			var caseModel = CaseMapping.Map(caseDto, StatusEnum.Close.ToString());

			// assert
			Assert.That(caseModel, Is.Not.Null);
			Assert.That(caseModel.Description, Is.EqualTo(caseDto.Description));
			Assert.That(caseModel.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(caseModel.StatusId, Is.EqualTo(caseDto.StatusId));
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
			Assert.That(caseModel.Territory, Is.EqualTo(caseDto.Territory));
			caseModel.IsArchived.Should().BeFalse();
			caseModel.Division.Should().Be(caseDto.Division);
			caseModel.Region.Should().Be(caseDto.Region);
			Assert.That(caseModel.Location, Is.EqualTo(location));
		}

		[Test]
		public void WhenMapCaseDto_CaseArchived_Returns_CaseModel()
		{
			var caseDto = CaseFactory.BuildCaseDto();
			caseDto.Urn = 1234567;

			var caseModel = CaseMapping.Map(caseDto, StatusEnum.Close.ToString());

			caseModel.IsArchived.Should().BeTrue();
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
			Assert.That(expectedCaseDto.StatusId, Is.EqualTo(statusDto.Id));
			Assert.That(expectedCaseDto.Urn, Is.EqualTo(caseDto.Urn));
			Assert.That(expectedCaseDto.ClosedAt, Is.Not.EqualTo(default(DateTimeOffset)));
			Assert.That(expectedCaseDto.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(expectedCaseDto.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(expectedCaseDto.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(expectedCaseDto.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(expectedCaseDto.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(expectedCaseDto.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(expectedCaseDto.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(expectedCaseDto.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(expectedCaseDto.ReviewAt, Is.Not.EqualTo(default(DateTimeOffset)));
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
			Assert.That(expectedCaseDto.StatusId, Is.EqualTo(statusDto.Id));
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
			Assert.That(expectedCaseDto.StatusId, Is.EqualTo(caseDto.StatusId));
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
			Assert.That(expectedCaseDto.StatusId, Is.EqualTo(caseDto.StatusId));
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
			Assert.That(expectedCaseDto.StatusId, Is.EqualTo(caseDto.StatusId));
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
			Assert.That(expectedCaseDto.StatusId, Is.EqualTo(caseDto.StatusId));
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
			Assert.That(expectedCaseDto.StatusId, Is.EqualTo(caseDto.StatusId));
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
			Assert.That(expectedCaseDto.StatusId, Is.EqualTo(caseDto.StatusId));
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
	}
}
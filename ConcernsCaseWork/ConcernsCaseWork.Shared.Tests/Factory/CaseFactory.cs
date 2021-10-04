using AutoFixture;
using ConcernsCaseWork.Models;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class CaseFactory
	{
		public static List<CaseDto> BuildListCaseDto(string trustUkPrn = null)
		{
			Fixture fixture = new Fixture();
			var dateTimeNow = DateTimeOffset.Now;
			return new List<CaseDto>
			{
				// Status
				// 1 - Live
				// 2 - Monitoring
				// 3 - Close
				new CaseDto(
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					fixture.Create<string>(),
					fixture.Create<string>(), 
					fixture.Create<string>(),
					trustUkPrn ?? fixture.Create<string>(), 
					fixture.Create<string>(), 
					dateTimeNow, 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					1, 
					1
				),
				new CaseDto(
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					fixture.Create<string>(),
					fixture.Create<string>(), 
					fixture.Create<string>(),
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					dateTimeNow, 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					2, 
					3
				),
				new CaseDto(
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					fixture.Create<string>(),
					fixture.Create<string>(), 
					fixture.Create<string>(),
					trustUkPrn ?? fixture.Create<string>(), 
					fixture.Create<string>(), 
					dateTimeNow, 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					3, 
					2
				),
				new CaseDto(
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					fixture.Create<string>(),
					fixture.Create<string>(), 
					fixture.Create<string>(),
					trustUkPrn ?? fixture.Create<string>(), 
					fixture.Create<string>(), 
					dateTimeNow, 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					4, 
					1
				),
				new CaseDto(
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					fixture.Create<string>(),
					fixture.Create<string>(), 
					fixture.Create<string>(),
					trustUkPrn ?? fixture.Create<string>(), 
					fixture.Create<string>(), 
					dateTimeNow, 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					5, 
					2
				)
			};
		}

		public static CaseDto BuildCaseDto()
		{
			Fixture fixture = new Fixture();
			var dateTimeNow = DateTime.Now;
			return new CaseDto(
				dateTimeNow, 
				dateTimeNow, 
				dateTimeNow, 
				dateTimeNow, 
				fixture.Create<string>(), 
				fixture.Create<string>(), 
				fixture.Create<string>(),
				fixture.Create<string>(), 
				fixture.Create<string>(), 
				dateTimeNow, 
				fixture.Create<string>(), 
				fixture.Create<string>(), 
				fixture.Create<string>(),
				fixture.Create<string>(), 
				fixture.Create<string>(), 
				fixture.Create<string>(), 
				1, 
				1
			);
		}
		
		public static CreateCaseDto BuildCreateCaseDto()
		{
			Fixture fixture = new Fixture();
			var dateTimeNow = DateTime.Now;
			return new CreateCaseDto(
				dateTimeNow, 
				dateTimeNow, 
				dateTimeNow, 
				dateTimeNow, 
				fixture.Create<string>(), 
				fixture.Create<string>(), 
				fixture.Create<string>(),
				fixture.Create<string>(), 
				fixture.Create<string>(), 
				dateTimeNow, 
				fixture.Create<string>(), 
				fixture.Create<string>(), 
				fixture.Create<string>(),
				fixture.Create<string>(), 
				fixture.Create<string>(), 
				fixture.Create<string>(), 
				1, 
				1
			);
		}
		
		public static CreateCaseModel BuildCreateCaseModel(string caseType = "case-type", string caseSubType = "case-sub-type")
		{
			Fixture fixture = new Fixture();
			var dateTimeNow = DateTime.Now;
			return new CreateCaseModel {
				CreatedAt = dateTimeNow, 
				UpdatedAt = dateTimeNow, 
				ReviewAt = dateTimeNow, 
				ClosedAt = dateTimeNow, 
				CreatedBy = fixture.Create<string>(), 
				Description = fixture.Create<string>(), 
				CrmEnquiry = fixture.Create<string>(),
				TrustUkPrn = fixture.Create<string>(), 
				ReasonAtReview = fixture.Create<string>(), 
				DeEscalation = dateTimeNow, 
				Issue = fixture.Create<string>(), 
				CurrentStatus = fixture.Create<string>(), 
				NextSteps = fixture.Create<string>(),
				CaseAim = fixture.Create<string>(),
				DeEscalationPoint = fixture.Create<string>(),
				DirectionOfTravel = fixture.Create<string>(), 
				Urn = 1, 
				Status = 1, 
				CaseType = caseType, 
				CaseSubType = caseSubType, 
				RagRatingName = fixture.Create<string>()
			};
		}
		
		public static CaseModel BuildCaseModel(string caseType = "case-type", string caseSubType = "case-sub-type")
		{
			Fixture fixture = new Fixture();
			var dateTimeNow = DateTimeOffset.Now;
			return new CaseModel
			{
				CreatedAt = dateTimeNow,
				UpdatedAt = dateTimeNow,
				ReviewAt = dateTimeNow,
				ClosedAt = dateTimeNow,
				CreatedBy = fixture.Create<string>(),
				Description = fixture.Create<string>(),
				CrmEnquiry = fixture.Create<string>(),
				TrustUkPrn = fixture.Create<string>(),
				ReasonAtReview = fixture.Create<string>(),
				DeEscalation = dateTimeNow,
				Issue = fixture.Create<string>(),
				CurrentStatus = fixture.Create<string>(),
				NextSteps = fixture.Create<string>(),
				CaseAim = fixture.Create<string>(),
				DeEscalationPoint = fixture.Create<string>(),
				DirectionOfTravel = fixture.Create<string>(),
				Urn = 1,
				Status = 1,
				StatusName = fixture.Create<string>(),
				CaseType = caseType,
				CaseSubType = caseSubType
			};
		}

		public static PatchCaseModel BuildPatchCaseModel()
		{
			Fixture fixture = new Fixture();
			return new PatchCaseModel
			{
				Urn = 1,
				CreatedBy = fixture.Create<string>(),
				CaseType = fixture.Create<string>(),
				TypeUrn = 1,
				UpdatedAt = DateTimeOffset.Now,
				CaseSubType = fixture.Create<string>(),
				RatingUrn = 1,
				RiskRating = fixture.Create<string>(),
				DirectionOfTravel = fixture.Create<string>(),
				Issue = fixture.Create<string>(),
				CurrentStatus = fixture.Create<string>(),
				CaseAim = fixture.Create<string>(),
				DeEscalationPoint = fixture.Create<string>()
			};
		}
	}
}
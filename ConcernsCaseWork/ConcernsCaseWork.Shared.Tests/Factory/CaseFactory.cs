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
		private readonly static Fixture Fixture = new Fixture();
		
		public static List<CaseDto> BuildListCaseDto(string trustUkPrn = null)
		{
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
					Fixture.Create<string>(),
					Fixture.Create<string>(), 
					Fixture.Create<string>(),
					trustUkPrn ?? Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					dateTimeNow, 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					1, 
					1
				),
				new CaseDto(
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					Fixture.Create<string>(),
					Fixture.Create<string>(), 
					Fixture.Create<string>(),
					trustUkPrn ?? Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					dateTimeNow, 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					2, 
					3
				),
				new CaseDto(
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					Fixture.Create<string>(),
					Fixture.Create<string>(), 
					Fixture.Create<string>(),
					trustUkPrn ?? Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					dateTimeNow, 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					3, 
					2
				),
				new CaseDto(
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					Fixture.Create<string>(),
					Fixture.Create<string>(), 
					Fixture.Create<string>(),
					trustUkPrn ?? Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					dateTimeNow, 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					4, 
					1
				),
				new CaseDto(
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					dateTimeNow, 
					Fixture.Create<string>(),
					Fixture.Create<string>(), 
					Fixture.Create<string>(),
					trustUkPrn ?? Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					dateTimeNow, 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					Fixture.Create<string>(), 
					5, 
					2
				)
			};
		}

		public static CaseDto BuildCaseDto()
		{
			var dateTimeNow = DateTime.Now;
			return new CaseDto(
				dateTimeNow, 
				dateTimeNow, 
				dateTimeNow, 
				dateTimeNow, 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(),
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				dateTimeNow, 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(),
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				1, 
				1
			);
		}
		
		public static CreateCaseDto BuildCreateCaseDto()
		{
			var dateTimeNow = DateTime.Now;
			return new CreateCaseDto(
				dateTimeNow, 
				dateTimeNow, 
				dateTimeNow, 
				dateTimeNow, 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(),
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				dateTimeNow, 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(),
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				1, 
				1
			);
		}
		
		public static CreateCaseModel BuildCreateCaseModel(string caseType = "case-type", string caseSubType = "case-sub-type")
		{
			var dateTimeNow = DateTime.Now;
			return new CreateCaseModel {
				CreatedAt = dateTimeNow, 
				UpdatedAt = dateTimeNow, 
				ReviewAt = dateTimeNow, 
				ClosedAt = dateTimeNow, 
				CreatedBy = Fixture.Create<string>(), 
				Description = Fixture.Create<string>(), 
				CrmEnquiry = Fixture.Create<string>(),
				TrustUkPrn = Fixture.Create<string>(), 
				ReasonAtReview = Fixture.Create<string>(), 
				DeEscalation = dateTimeNow, 
				Issue = Fixture.Create<string>(), 
				CurrentStatus = Fixture.Create<string>(), 
				NextSteps = Fixture.Create<string>(),
				CaseAim = Fixture.Create<string>(),
				DeEscalationPoint = Fixture.Create<string>(),
				DirectionOfTravel = Fixture.Create<string>(), 
				Urn = 1, 
				Status = 1, 
				Type = caseType, 
				SubType = caseSubType, 
				RagRatingName = Fixture.Create<string>()
			};
		}
		
		public static CaseModel BuildCaseModel(string caseType = "case-type", string caseSubType = "case-sub-type", string createdBy = "created-user")
		{
			var dateTimeNow = DateTimeOffset.Now;
			return new CaseModel
			{
				CreatedAt = dateTimeNow,
				UpdatedAt = dateTimeNow,
				ReviewAt = dateTimeNow,
				ClosedAt = dateTimeNow,
				CreatedBy = createdBy,
				Description = Fixture.Create<string>(),
				CrmEnquiry = Fixture.Create<string>(),
				TrustUkPrn = Fixture.Create<string>(),
				ReasonAtReview = Fixture.Create<string>(),
				DeEscalation = dateTimeNow,
				Issue = Fixture.Create<string>(),
				CurrentStatus = Fixture.Create<string>(),
				NextSteps = Fixture.Create<string>(),
				CaseAim = Fixture.Create<string>(),
				DeEscalationPoint = Fixture.Create<string>(),
				DirectionOfTravel = Fixture.Create<string>(),
				Urn = 1,
				Status = 1,
				StatusName = Fixture.Create<string>(),
				CaseType = caseType,
				CaseSubType = caseSubType,
				TrustDetailsModel = Fixture.Create<TrustDetailsModel>()
			};
		}

		public static PatchCaseModel BuildPatchCaseModel()
		{
			return new PatchCaseModel
			{
				Urn = 1,
				CreatedBy = Fixture.Create<string>(),
				CaseType = Fixture.Create<string>(),
				TypeUrn = 1,
				UpdatedAt = DateTimeOffset.Now,
				CaseSubType = Fixture.Create<string>(),
				RatingUrn = 1,
				RiskRating = Fixture.Create<string>(),
				DirectionOfTravel = Fixture.Create<string>(),
				Issue = Fixture.Create<string>(),
				CurrentStatus = Fixture.Create<string>(),
				CaseAim = Fixture.Create<string>(),
				DeEscalationPoint = Fixture.Create<string>()
			};
		}
	}
}
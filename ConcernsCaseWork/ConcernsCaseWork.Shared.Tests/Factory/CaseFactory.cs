using AutoFixture;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Status;
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
				new CaseDto
				{
					CreatedAt = dateTimeNow,
					UpdatedAt = dateTimeNow,
					ReviewAt = dateTimeNow,
					ClosedAt = dateTimeNow,
					CreatedBy = Fixture.Create<string>(),
					Description = Fixture.Create<string>(),
					CrmEnquiry = Fixture.Create<string>(),
					TrustUkPrn = trustUkPrn ?? Fixture.Create<string>(),
					ReasonAtReview = Fixture.Create<string>(),
					DeEscalation = dateTimeNow,
					Issue = Fixture.Create<string>(),
					CurrentStatus = Fixture.Create<string>(),
					CaseAim = Fixture.Create<string>(),
					DeEscalationPoint = Fixture.Create<string>(),
					NextSteps = Fixture.Create<string>(),
					CaseHistory = Fixture.Create<string>(),
					Territory = Fixture.Create<TerritoryEnum>(),
					DirectionOfTravel = Fixture.Create<string>(),
					Urn = 1,
					StatusId = 1,
					RatingId = 1
				},
				new CaseDto
				{
					CreatedAt = dateTimeNow,
					UpdatedAt = dateTimeNow,
					ReviewAt = dateTimeNow,
					ClosedAt = dateTimeNow,
					CreatedBy = Fixture.Create<string>(),
					Description = Fixture.Create<string>(),
					CrmEnquiry = Fixture.Create<string>(),
					TrustUkPrn = trustUkPrn ?? Fixture.Create<string>(),
					ReasonAtReview = Fixture.Create<string>(),
					DeEscalation = dateTimeNow,
					Issue = Fixture.Create<string>(),
					CurrentStatus = Fixture.Create<string>(),
					CaseAim = Fixture.Create<string>(),
					DeEscalationPoint = Fixture.Create<string>(),
					NextSteps = Fixture.Create<string>(),
					CaseHistory = Fixture.Create<string>(),
					Territory = Fixture.Create<TerritoryEnum>(),
					DirectionOfTravel = Fixture.Create<string>(),
					Urn = 2,
					StatusId = 3,
					RatingId = 2
				},
				new CaseDto
				{
					CreatedAt = dateTimeNow,
					UpdatedAt = dateTimeNow,
					ReviewAt = dateTimeNow,
					ClosedAt = dateTimeNow,
					CreatedBy = Fixture.Create<string>(),
					Description = Fixture.Create<string>(),
					CrmEnquiry = Fixture.Create<string>(),
					TrustUkPrn = trustUkPrn ?? Fixture.Create<string>(),
					ReasonAtReview = Fixture.Create<string>(),
					DeEscalation = dateTimeNow,
					Issue = Fixture.Create<string>(),
					CurrentStatus = Fixture.Create<string>(),
					CaseAim = Fixture.Create<string>(),
					DeEscalationPoint = Fixture.Create<string>(),
					NextSteps = Fixture.Create<string>(),
					CaseHistory = Fixture.Create<string>(),
					Territory = Fixture.Create<TerritoryEnum>(),
					DirectionOfTravel = Fixture.Create<string>(),
					Urn = 3,
					StatusId = 2,
					RatingId = 3
				},
				new CaseDto
				{
					CreatedAt = dateTimeNow,
					UpdatedAt = dateTimeNow,
					ReviewAt = dateTimeNow,
					ClosedAt = dateTimeNow,
					CreatedBy = Fixture.Create<string>(),
					Description = Fixture.Create<string>(),
					CrmEnquiry = Fixture.Create<string>(),
					TrustUkPrn = trustUkPrn ?? Fixture.Create<string>(),
					ReasonAtReview = Fixture.Create<string>(),
					DeEscalation = dateTimeNow,
					Issue = Fixture.Create<string>(),
					CurrentStatus = Fixture.Create<string>(),
					CaseAim = Fixture.Create<string>(),
					DeEscalationPoint = Fixture.Create<string>(),
					NextSteps = Fixture.Create<string>(),
					CaseHistory = Fixture.Create<string>(),
					Territory = Fixture.Create<TerritoryEnum>(),
					DirectionOfTravel = Fixture.Create<string>(),
					Urn = 4,
					StatusId = 1,
					RatingId = 4
				},
				new CaseDto
				{
					CreatedAt = dateTimeNow,
					UpdatedAt = dateTimeNow,
					ReviewAt = dateTimeNow,
					ClosedAt = dateTimeNow,
					CreatedBy = Fixture.Create<string>(),
					Description = Fixture.Create<string>(),
					CrmEnquiry = Fixture.Create<string>(),
					TrustUkPrn = trustUkPrn ?? Fixture.Create<string>(),
					ReasonAtReview = Fixture.Create<string>(),
					DeEscalation = dateTimeNow,
					Issue = Fixture.Create<string>(),
					CurrentStatus = Fixture.Create<string>(),
					CaseAim = Fixture.Create<string>(),
					DeEscalationPoint = Fixture.Create<string>(),
					NextSteps = Fixture.Create<string>(),
					CaseHistory = Fixture.Create<string>(),
					Territory = Fixture.Create<TerritoryEnum>(),
					DirectionOfTravel = Fixture.Create<string>(),
					Urn = 5,
					StatusId = 2,
					RatingId = 5
				}
			};
		}
		
		public static CaseDto BuildCaseDto()
			=> new CaseDto
				{
					CreatedAt = Fixture.Create<DateTimeOffset>(),
					UpdatedAt = Fixture.Create<DateTimeOffset>(),
					ReviewAt = Fixture.Create<DateTimeOffset>(),
					ClosedAt = Fixture.Create<DateTimeOffset>(),
					CreatedBy = Fixture.Create<string>(),
					Description = Fixture.Create<string>(),
					CrmEnquiry = Fixture.Create<string>(),
					TrustUkPrn = Fixture.Create<string>(),
					ReasonAtReview = Fixture.Create<string>(),
					DeEscalation = Fixture.Create<DateTimeOffset>(),
					Issue = Fixture.Create<string>(),
					CurrentStatus = Fixture.Create<string>(),
					CaseAim = Fixture.Create<string>(),
					DeEscalationPoint = Fixture.Create<string>(),
					NextSteps = Fixture.Create<string>(),
					CaseHistory = Fixture.Create<string>(),
					Territory = Fixture.Create<TerritoryEnum>(),
					DirectionOfTravel = Fixture.Create<string>(),
					Urn = 1,
					StatusId = 1,
					RatingId = 1
				};
		
		public static CreateCaseDto BuildCreateCaseDto(string createdBy = null, string trustUkprn = null)
		{
			var dateTimeNow = DateTime.Now;
			return new CreateCaseDto(
				dateTimeNow, 
				dateTimeNow, 
				dateTimeNow, 
				dateTimeNow, 
				createdBy ?? Fixture.Create<string>(), 
				Fixture.Create<string>(),
				trustUkprn ?? Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				dateTimeNow, 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(),
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(),
				1,
				2,
				Fixture.Create<TerritoryEnum>()
			);
		}
		
		public static CreateCaseModel BuildCreateCaseModel()
		{
			var dateTimeNow = DateTime.Now;
			return new CreateCaseModel {
				CreatedAt = dateTimeNow, 
				UpdatedAt = dateTimeNow, 
				ReviewAt = dateTimeNow, 
				ClosedAt = dateTimeNow, 
				CreatedBy = Fixture.Create<string>(), 
				CrmEnquiry = Fixture.Create<string>(),
				TrustUkPrn = Fixture.Create<string>(), 
				ReasonAtReview = Fixture.Create<string>(), 
				DeEscalation = dateTimeNow, 
				Issue = Fixture.Create<string>(), 
				CurrentStatus = Fixture.Create<string>(), 
				NextSteps = Fixture.Create<string>(),
				CaseAim = Fixture.Create<string>(),
				CaseHistory = Fixture.Create<string>(),
				DeEscalationPoint = Fixture.Create<string>(),
				DirectionOfTravel = Fixture.Create<string>(),
				StatusId = 1
			};
		}
		
		public static CaseModel BuildCaseModel(string createdBy = "created-user", long statusId = 1)
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
				CaseHistory = Fixture.Create<string>(),
				Territory = Fixture.Create<TerritoryEnum>(),
				DeEscalationPoint = Fixture.Create<string>(),
				DirectionOfTravel = Fixture.Create<string>(),
				Urn = 1,
				StatusId = statusId,
				StatusName = Fixture.Create<string>()
			};
		}

		public static PatchCaseModel BuildPatchCaseModel()
		{
			return new PatchCaseModel
			{
				Urn = 1,
				CreatedBy = Fixture.Create<string>(),
				UpdatedAt = Fixture.Create<DateTimeOffset>(),
				RatingId = 1,
				DirectionOfTravel = Fixture.Create<string>(),
				Issue = Fixture.Create<string>(),
				CurrentStatus = Fixture.Create<string>(),
				CaseAim = Fixture.Create<string>(),
				DeEscalationPoint = Fixture.Create<string>(),
				NextSteps = Fixture.Create<string>(),
				CaseHistory = Fixture.Create<string>(),
				StatusName = Fixture.Create<string>()
			};
		}

		public static IList<TrustCasesModel> BuildListTrustCasesModel()
		{
			return new List<TrustCasesModel>
			{
				new TrustCasesModel(
					Fixture.Create<long>(),
					Fixture.Create<IList<RecordModel>>(),
					Fixture.Create<RatingModel>(),
					Fixture.Create<DateTimeOffset>(),
					Fixture.Create<DateTimeOffset>(),
					Fixture.Create<StatusEnum>()
					)
			};
		}
		
		public static CaseTrustSearch BuildCaseTrustSearch(string trustUkPrn = "")
		{
			return new CaseTrustSearch(trustUkPrn);
		}

		public static CaseCaseWorkerSearch BuildCaseCaseWorkerSearch()
		{
			return new CaseCaseWorkerSearch(Fixture.Create<string>(), Fixture.Create<long>());
		}
		
		public static PageSearch BuildPageSearch()
		{
			return new PageSearch();
		}
		
		public static CaseSearch BuildCaseSearch(long caseUrn = 1)
		{
			return new CaseSearch(caseUrn);
		}
	}
}
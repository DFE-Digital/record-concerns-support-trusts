﻿using AutoFixture;
using ConcernsCaseWork.Models;
using Service.Redis.Models;
using ConcernsCasework.Service.Base;
using ConcernsCasework.Service.Cases;
using ConcernsCasework.Service.Status;
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
					3, 
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
					4, 
					1,
					4
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
					2,
					5
				)
			};
		}
		
		public static CaseDto BuildCaseDto()
		{
			return new CaseDto(
				Fixture.Create<DateTimeOffset>(), 
				Fixture.Create<DateTimeOffset>(), 
				Fixture.Create<DateTimeOffset>(), 
				Fixture.Create<DateTimeOffset>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(),
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<DateTimeOffset>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(),
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				1, 
				1,
				1
			);
		}
		
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
				1,
				2
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
				DeEscalationPoint = Fixture.Create<string>(),
				DirectionOfTravel = Fixture.Create<string>(),
				StatusUrn = 1
			};
		}
		
		public static CaseModel BuildCaseModel(string createdBy = "created-user", long statusUrn = 1)
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
				StatusUrn = statusUrn,
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
				RatingUrn = 1,
				DirectionOfTravel = Fixture.Create<string>(),
				Issue = Fixture.Create<string>(),
				CurrentStatus = Fixture.Create<string>(),
				CaseAim = Fixture.Create<string>(),
				DeEscalationPoint = Fixture.Create<string>(),
				NextSteps = Fixture.Create<string>(),
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

		public static IList<CaseHistoryDto> BuildListCasesHistoryDto()
		{
			return new List<CaseHistoryDto>
			{
				BuildCaseHistoryDto()
			};
		}

		public static IList<CaseHistoryModel> BuildListCasesHistoryModel()
		{
			return new List<CaseHistoryModel>
			{
				new CaseHistoryModel {
					CreatedAt = Fixture.Create<DateTimeOffset>(),
					Urn = Fixture.Create<long>(),
					CaseUrn = Fixture.Create<long>(),
					Action = Fixture.Create<string>(),
					Description = Fixture.Create<string>(),
					Title = Fixture.Create<string>()
				}
			};
		}
		
		public static CaseHistoryDto BuildCaseHistoryDto()
		{
			return new CaseHistoryDto(
				Fixture.Create<DateTimeOffset>(),
				Fixture.Create<long>(),
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				Fixture.Create<long>());
		}

		public static CreateCaseHistoryDto BuildCreateCaseHistoryDto(long caseUrn = 1)
		{
			return new CreateCaseHistoryDto(
				Fixture.Create<DateTimeOffset>(),
				caseUrn,
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				Fixture.Create<string>());
		}
	}
}
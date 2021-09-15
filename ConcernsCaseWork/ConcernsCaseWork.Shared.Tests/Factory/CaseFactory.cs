using ConcernsCaseWork.Models;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class CaseFactory
	{
		public static List<CaseDto> BuildListCaseDto()
		{
			var dateTimeNow = DateTimeOffset.Now;
			return new List<CaseDto>
			{
				// Status
				// 1 - Live
				// 2 - Monitoring
				// 3 - Close
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", 1, 1
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", 2, 3
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", 3, 2
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", 4, 1
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", 5, 2
				)
			};
		}

		public static CaseDto BuildCaseDto()
		{
			var dateTimeNow = DateTime.Now;
			return new CaseDto(
				dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
				"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
				"resolution-strategy", "direction-of-travel", 1, 1
			);
		}
		
		public static CreateCaseDto BuildCreateCaseDto()
		{
			var dateTimeNow = DateTime.Now;
			return new CreateCaseDto(
				dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
				"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
				"resolution-strategy", "direction-of-travel", 1, 1
			);
		}
		
		public static CaseModel BuildCaseModel()
		{
			var dateTimeNow = DateTimeOffset.Now;
			return new CaseModel
			{
				CreatedAt = dateTimeNow,
				UpdatedAt = dateTimeNow,
				ReviewAt = dateTimeNow,
				ClosedAt = dateTimeNow,
				CreatedBy = "testing",
				Description = "description",
				CrmEnquiry = "crm-enquiry",
				TrustUkPrn = "trust-ukprn",
				ReasonAtReview = "reason-at-review",
				DeEscalation = dateTimeNow,
				Issue = "issue",
				CurrentStatus = "current-status",
				NextSteps = "nextSteps",
				ResolutionStrategy = "resolution-strategy",
				DirectionOfTravel = "direction-of-travel",
				Urn = 1,
				Status = 1,
				StatusName = "Live"
			};
		}
		
		public static IEnumerable<CaseModel> BuildListCaseModels()
		{
			var dateTimeNow = DateTimeOffset.Now;
			return new List<CaseModel>
			{
				new CaseModel
				{
					CreatedAt = dateTimeNow,
					UpdatedAt = dateTimeNow,
					ReviewAt = dateTimeNow,
					ClosedAt = dateTimeNow,
					CreatedBy = "testing",
					Description = "description",
					CrmEnquiry = "crm-enquiry",
					TrustUkPrn = "trust-ukprn",
					ReasonAtReview = "reason-at-review",
					DeEscalation = dateTimeNow,
					Issue = "issue",
					CurrentStatus = "current-status",
					NextSteps = "nextSteps",
					ResolutionStrategy = "resolution-strategy",
					DirectionOfTravel = "direction-of-travel",
					Urn = 1,
					Status = 1,
					StatusName = "Live"
				},
				new CaseModel				
				{
					CreatedAt = dateTimeNow,
					UpdatedAt = dateTimeNow,
					ReviewAt = dateTimeNow,
					ClosedAt = dateTimeNow,
					CreatedBy = "testing",
					Description = "description",
					CrmEnquiry = "crm-enquiry",
					TrustUkPrn = "trust-ukprn",
					ReasonAtReview = "reason-at-review",
					DeEscalation = dateTimeNow,
					Issue = "issue",
					CurrentStatus = "current-status",
					NextSteps = "nextSteps",
					ResolutionStrategy = "resolution-strategy",
					DirectionOfTravel = "direction-of-travel",
					Urn = 2,
					Status = 1,
					StatusName = "Live"
				},
				new CaseModel
				{
					CreatedAt = dateTimeNow,
					UpdatedAt = dateTimeNow,
					ReviewAt = dateTimeNow,
					ClosedAt = dateTimeNow,
					CreatedBy = "testing",
					Description = "description",
					CrmEnquiry = "crm-enquiry",
					TrustUkPrn = "trust-ukprn",
					ReasonAtReview = "reason-at-review",
					DeEscalation = dateTimeNow,
					Issue = "issue",
					CurrentStatus = "current-status",
					NextSteps = "nextSteps",
					ResolutionStrategy = "resolution-strategy",
					DirectionOfTravel = "direction-of-travel",
					Urn = 3,
					Status = 3,
					StatusName = "Close"
				},
				new CaseModel
				{
					CreatedAt = dateTimeNow,
					UpdatedAt = dateTimeNow,
					ReviewAt = dateTimeNow,
					ClosedAt = dateTimeNow,
					CreatedBy = "testing",
					Description = "description",
					CrmEnquiry = "crm-enquiry",
					TrustUkPrn = "trust-ukprn",
					ReasonAtReview = "reason-at-review",
					DeEscalation = dateTimeNow,
					Issue = "issue",
					CurrentStatus = "current-status",
					NextSteps = "nextSteps",
					ResolutionStrategy = "resolution-strategy",
					DirectionOfTravel = "direction-of-travel",
					Urn = 4,
					Status = 2,
					StatusName = "Monitoring"
				},
				new CaseModel
				{
					CreatedAt = dateTimeNow,
					UpdatedAt = dateTimeNow,
					ReviewAt = dateTimeNow,
					ClosedAt = dateTimeNow,
					CreatedBy = "testing",
					Description = "description",
					CrmEnquiry = "crm-enquiry",
					TrustUkPrn = "trust-ukprn",
					ReasonAtReview = "reason-at-review",
					DeEscalation = dateTimeNow,
					Issue = "issue",
					CurrentStatus = "current-status",
					NextSteps = "nextSteps",
					ResolutionStrategy = "resolution-strategy",
					DirectionOfTravel = "direction-of-travel",
					Urn = 5,
					Status = 2,
					StatusName = "Monitoring"
				},
			};
		}
	}
}
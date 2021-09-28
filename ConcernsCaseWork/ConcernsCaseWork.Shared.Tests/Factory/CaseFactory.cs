using ConcernsCaseWork.Models;
using Service.Redis.Models;
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
					"case-aim", "de-escalation-point", "direction-of-travel", 1, 1
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"case-aim", "de-escalation-point", "direction-of-travel", 2, 3
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"case-aim", "de-escalation-point", "direction-of-travel", 3, 2
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"case-aim", "de-escalation-point", "direction-of-travel", 4, 1
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"case-aim", "de-escalation-point", "direction-of-travel", 5, 2
				)
			};
		}

		public static CaseDto BuildCaseDto()
		{
			var dateTimeNow = DateTime.Now;
			return new CaseDto(
				dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
				"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
				"case-aim", "de-escalation-point", "direction-of-travel", 1, 1
			);
		}
		
		public static CreateCaseDto BuildCreateCaseDto()
		{
			var dateTimeNow = DateTime.Now;
			return new CreateCaseDto(
				dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
				"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
				"case-aim", "de-escalation-point", "direction-of-travel", 1, 1
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
				CreatedBy = "testing", 
				Description = "description", 
				CrmEnquiry = "crm-enquiry",
				TrustUkPrn = "trust-ukprn", 
				ReasonAtReview = "reason-at-review", 
				DeEscalation = dateTimeNow, 
				Issue = "issue", 
				CurrentStatus = "current-status", 
				NextSteps = "nextSteps",
				CaseAim = "case-aim",
				DeEscalationPoint = "de-escalation-point",
				DirectionOfTravel = "direction-of-travel", 
				Urn = 1, 
				Status = 1, 
				RecordType = "record-type", 
				RecordSubType = "record-sub-.type", 
				RagRating = "rag-rating"
			};
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
				CaseAim = "case-aim",
				DeEscalationPoint = "de-escalation-point",
				DirectionOfTravel = "direction-of-travel",
				Urn = 1,
				Status = 1,
				StatusName = "Live"
			};
		}

		public static PatchCaseModel BuildPatchCaseModel()
		{
			return new PatchCaseModel
			{
				Urn = 1,
				CreatedBy = "testing",
				RecordType = "record-type",
				TypeUrn = 1,
				UpdatedAt = DateTimeOffset.Now,
				RecordSubType = "record-sub-type",
				RatingUrn = 1,
				RiskRating = "Amber Green"
			};
		}
	}
}
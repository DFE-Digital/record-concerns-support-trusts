using ConcernsCaseWork.Models;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class CaseModelFactory
	{
		public static List<CaseModel> BuildCaseModels()
		{
			var dateTimeNow = DateTime.Now;
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
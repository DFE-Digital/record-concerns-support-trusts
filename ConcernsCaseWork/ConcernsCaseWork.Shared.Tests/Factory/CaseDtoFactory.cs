using Service.TRAMS.Cases;
using Service.TRAMS.Sequence;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class CaseDtoFactory
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
					"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 1
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 3
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 2
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 1
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 2
				)
			};
		}

		public static CaseDto BuildCaseDto()
		{
			var dateTimeNow = DateTime.Now;
			return new CaseDto(
				dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
				"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
				"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 1
			);
		}
		
		public static CreateCaseDto BuildCreateCaseDto()
		{
			var dateTimeNow = DateTime.Now;
			return new CreateCaseDto(
				dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
				"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
				"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 1
			);
		}
	}
}
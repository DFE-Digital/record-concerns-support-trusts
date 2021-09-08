using ConcernsCaseWork.Shared.Tests.Shared;
using Service.TRAMS.Dto;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class CaseDtoFactory
	{
		public static List<CaseDto> CreateListCaseDto()
		{
			var dateTimeNow = DateTimeOffset.Now;
			return new List<CaseDto>
			{
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "trust-name", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Live"
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "trust-name", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Close"
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "trust-name", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Monitoring"
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "trust-name", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Live"
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "trust-name", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Monitoring"
				)
			};
		}

		public static CaseDto CreateCaseDto()
		{
			var dateTimeNow = DateTimeOffset.Now;
			return new CaseDto(
				dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
				"trust-ukprn", "trust-name", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
				"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Live"
			);
		}
	}
}
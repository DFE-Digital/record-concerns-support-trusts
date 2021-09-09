using ConcernsCaseWork.Shared.Tests.Shared;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class CaseDtoFactory
	{
		public static List<CaseDto> CreateListCaseDto()
		{
			var dateTimeNow = DateTime.Now;
			return new List<CaseDto>
			{
				// Status
				// 1 - Live
				// 2 - Monitoring
				// 3 - Close
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), new BigInteger(1)
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), new BigInteger(3)
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), new BigInteger(2)
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), new BigInteger(1)
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps", 
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), new BigInteger(2)
				)
			};
		}

		public static CaseDto CreateCaseDto()
		{
			var dateTimeNow = DateTime.Now;
			return new CaseDto(
				dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
				"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
				"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), new BigInteger(1)
			);
		}
	}
}
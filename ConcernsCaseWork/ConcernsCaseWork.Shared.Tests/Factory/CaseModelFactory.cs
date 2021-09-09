using ConcernsCaseWork.Models;
using ConcernsCaseWork.Shared.Tests.Shared;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class CaseModelFactory
	{
		public static List<CaseModel> CreateCaseModels()
		{
			var dateTimeNow = DateTime.Now;
			return new List<CaseModel>
			{
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Live"
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Live"
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Close"
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Monitoring"
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
					"resolution-strategy", "direction-of-travel", BigIntegerSequence.Generator(), "Monitoring"
				)
			};
		}
	}
}
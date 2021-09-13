using ConcernsCaseWork.Models;
using Service.TRAMS.Sequence;
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
					"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 1
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
					"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 1
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
					"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 3
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
					"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 2
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crm-enquiry",
					"trust-ukprn", "reason-at-review", dateTimeNow, "issue", "current-status", "nextSteps",
					"resolution-strategy", "direction-of-travel", LongSequence.Generator(), 2
				)
			};
		}
	}
}
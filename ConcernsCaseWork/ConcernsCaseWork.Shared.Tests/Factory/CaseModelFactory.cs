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
			var dateTimeNow = DateTimeOffset.Now;
			return new List<CaseModel>
			{
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigIntegerSequence.Generator(), "Live"
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigIntegerSequence.Generator(), "Live"
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigIntegerSequence.Generator(), "Close"
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigIntegerSequence.Generator(), "Monitoring"
				),
				new CaseModel(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigIntegerSequence.Generator(), "Monitoring"
				)			
			};
		}
	}
}
using ConcernsCaseWork.Models;
using System;
using System.Collections.Generic;
using System.Numerics;

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
					1, dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, Int32.MinValue
				),
				new CaseModel(
					2, dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, Int32.MinValue
				),
				new CaseModel(
					3, dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, Int32.MinValue
				),
				new CaseModel(
					4, dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, Int32.MinValue
				),
				new CaseModel(
					5, dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, Int32.MinValue
				)			
			};
		}
	}
}
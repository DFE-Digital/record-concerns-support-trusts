using Service.TRAMS.Dto;
using System;
using System.Collections.Generic;
using System.Numerics;

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
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, "Live"
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, "Close"
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, "Monitoring"
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, "Live"
				),
				new CaseDto(
					dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, "Monitoring"
				)
			};
		}

		public static CaseDto CreateCaseDto()
		{
			var dateTimeNow = DateTimeOffset.Now;
			return new CaseDto(
				dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing", "description", "crmenquiry",
				"trustukprn", "trustname", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps",
				"resolutionstrategy", "directionoftravel", BigInteger.One, "Live"
			);
		}
	}
}
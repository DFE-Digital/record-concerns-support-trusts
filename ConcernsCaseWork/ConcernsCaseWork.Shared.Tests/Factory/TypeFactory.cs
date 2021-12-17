using ConcernsCaseWork.Models;
using Service.TRAMS.Types;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class TypeFactory
	{
		public static List<TypeDto> BuildListTypeDto()
		{
			var currentDate = DateTimeOffset.Now;
			return new List<TypeDto>
			{
				new TypeDto("Compliance", "Compliance: Financial reporting", currentDate, 
					currentDate, 1),
				new TypeDto("Compliance", "Compliance: Financial returns", currentDate, 
					currentDate, 2),
				new TypeDto("Financial", "Financial: Deficit", currentDate, 
					currentDate, 3),
				new TypeDto("Financial", "Financial: Projected deficit / Low future surplus", currentDate, 
					currentDate, 4),
				new TypeDto("Financial", "Financial: Cash flow shortfall", currentDate, 
					currentDate, 5),
				new TypeDto("Financial", "Financial: Clawback", currentDate, 
					currentDate, 6),
				new TypeDto("Force Majeure", "", currentDate, 
					currentDate, 7),
				new TypeDto("Governance", "Governance: Governance", currentDate, 
					currentDate, 8),
				new TypeDto("Governance", "Governance: Closure", currentDate, 
					currentDate, 9),
				new TypeDto("Governance", "Governance: Executive Pay", currentDate, 
					currentDate, 10),
				new TypeDto("Governance", "Governance: Safeguarding", currentDate, 
					currentDate, 11),
				new TypeDto("Irregularity", "Irregularity: Allegations and self reported concerns", currentDate, 
					currentDate, 12),
				new TypeDto("Irregularity", "Irregularity: Related party transactions - in year", currentDate, 
					currentDate, 13)
			};
		}

		public static List<TypeDto> BuildListOrphanTypeDto()
		{
			var currentDate = DateTimeOffset.Now;
			return new List<TypeDto>
			{
				new TypeDto("Compliance", "Compliance: Financial reporting", currentDate,
					currentDate, 0)
			};
		}

		public static TypeModel BuildTypeModel()
		{
			return new TypeModel
			{
				Type = "Compliance",
				SubType = "Compliance: Financial reporting",
				TypesDictionary = new Dictionary<string, IList<TypeModel.TypeValueModel>>()
			};
		}
	}
}
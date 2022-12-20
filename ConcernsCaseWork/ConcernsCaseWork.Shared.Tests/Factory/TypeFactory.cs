using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Types;
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
				new TypeDto("Financial", "Financial: Deficit", currentDate, 
					currentDate, 3),
				new TypeDto("Financial", "Financial: Projected deficit", currentDate, 
					currentDate, 4),
				new TypeDto("Financial", "Financial: Viability", currentDate, 
					currentDate, 20),
				
				new TypeDto("Force Majeure", "", currentDate, 
					currentDate, 7),
				
				new TypeDto("Governance and compliance", "Governance and compliance: Governance", currentDate, 
					currentDate, 8),
				new TypeDto("Governance and compliance", "Governance and compliance: Compliance", currentDate, 
					currentDate, 23),

				new TypeDto("Irregularity", "Irregularity: Irregularity", currentDate, 
					currentDate, 21),
				new TypeDto("Irregularity", "Irregularity: Suspected fraud", currentDate, 
					currentDate, 22)
			};
		}

		public static TypeModel BuildTypeModel()
		{
			return new TypeModel
			{
				Type = "Financial",
				SubType = "Financial: Deficit",
				TypesDictionary = new Dictionary<string, IList<TypeModel.TypeValueModel>>()
			};
		}
	}
}
using ConcernsCaseWork.API.Contracts.Enums;
using System;
namespace ConcernsCaseWork.API.Contracts.Decisions
{
	public class DecisionTypeQuestion
	{
		public DecisionType Id { get; set; }
		public DecisionDrawdownFacilityAgreed? DecisionDrawdownFacilityAgreedId { get; set; }
		public DecisionFrameworkCategory? DecisionFrameworkCategoryId { get; set; }

		public DecisionTypeQuestion()
		{
		}
	}
}


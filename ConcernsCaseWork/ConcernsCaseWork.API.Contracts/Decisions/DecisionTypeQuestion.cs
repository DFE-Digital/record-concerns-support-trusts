using System;
namespace ConcernsCaseWork.API.Contracts.Decisions
{
	public class DecisionTypeQuestion
	{
		public DecisionType Id { get; set; }
		public DrawdownFacilityAgreed? DecisionDrawdownFacilityAgreedId { get; set; }
		public FrameworkCategory? DecisionFrameworkCategoryId { get; set; }
		public DrawdownFacilityAgreed? DecisionFinancialSupportPackageTypeId { get; set; }

		public DecisionTypeQuestion()
		{
		}
	}
}


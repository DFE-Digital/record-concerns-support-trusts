namespace ConcernsCaseWork.Data.Models.Decisions
{
	public class DecisionType
	{
		public API.Contracts.Decisions.DecisionType DecisionTypeId { get; set; }
		public API.Contracts.Decisions.DrawdownFacilityAgreed? DecisionDrawdownFacilityAgreedId { get; set; }
		public API.Contracts.Decisions.FrameworkCategory? DecisionFrameworkCategoryId { get; set; }
		public API.Contracts.Decisions.DrawdownFacilityAgreed? DecisionFinancialSupportPackageTypeId { get; set; }

		private DecisionType()
		{
		}

		public DecisionType(API.Contracts.Decisions.DecisionType decisionType, API.Contracts.Decisions.DrawdownFacilityAgreed? decisionDrawdownFacilityAgreed, API.Contracts.Decisions.FrameworkCategory? decisionFrameworkCategory, API.Contracts.Decisions.DrawdownFacilityAgreed? decisionFinancialSupportPackageType = null) : this()
		{
			if (!Enum.IsDefined(typeof(API.Contracts.Decisions.DecisionType), decisionType))
				throw new ArgumentOutOfRangeException(nameof(decisionType),
					$"{decisionType} value is not one of the supported decision types");

			if (decisionDrawdownFacilityAgreed.HasValue && !Enum.IsDefined(typeof(API.Contracts.Decisions.DrawdownFacilityAgreed), decisionDrawdownFacilityAgreed))
				throw new ArgumentOutOfRangeException(nameof(decisionDrawdownFacilityAgreed),
					$"{decisionDrawdownFacilityAgreed} value is not one of the supported decision drawdown facility agreed");


			if (decisionFrameworkCategory.HasValue && !Enum.IsDefined(typeof(API.Contracts.Decisions.FrameworkCategory), decisionFrameworkCategory))
				throw new ArgumentOutOfRangeException(nameof(decisionFrameworkCategory),
					$"{decisionFrameworkCategory} value is not one of the supported decision category");

			if (decisionFinancialSupportPackageType.HasValue && !Enum.IsDefined(typeof(API.Contracts.Decisions.DrawdownFacilityAgreed), decisionFinancialSupportPackageType))
				throw new ArgumentOutOfRangeException(nameof(decisionFinancialSupportPackageType),
					$"{decisionFinancialSupportPackageType} value is not one of the supported decision financial support package type");

			DecisionTypeId = decisionType;
			DecisionDrawdownFacilityAgreedId = decisionDrawdownFacilityAgreed;
			DecisionFrameworkCategoryId = decisionFrameworkCategory;
			DecisionFinancialSupportPackageTypeId = decisionFinancialSupportPackageType;
		}

		public int DecisionId { get; set; }
	}
}

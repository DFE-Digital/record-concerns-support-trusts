namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions
{
	public class DecisionType
	{
		public Enums.Concerns.DecisionType DecisionTypeId { get; set; }
		public Enums.Concerns.DecisionDrawdownFacilityAgreed? DecisionDrawdownFacilityAgreedId { get; set; }
		public Enums.Concerns.DecisionFrameworkCategory? DecisionFrameworkCategoryId { get; set; }

		private DecisionType()
		{
		}

		public DecisionType(Enums.Concerns.DecisionType decisionType, Enums.Concerns.DecisionDrawdownFacilityAgreed? decisionDrawdownFacilityAgreed, Enums.Concerns.DecisionFrameworkCategory? decisionFrameworkCategory) : this()
		{
			if (!Enum.IsDefined(typeof(Enums.Concerns.DecisionType), decisionType))
			{
				throw new ArgumentOutOfRangeException(nameof(decisionType),
					$"{decisionType} value is not one of the supported decision types");
			}

			if (decisionDrawdownFacilityAgreed.HasValue && !Enum.IsDefined(typeof(Enums.Concerns.DecisionDrawdownFacilityAgreed), decisionDrawdownFacilityAgreed))
			{
				throw new ArgumentOutOfRangeException(nameof(decisionDrawdownFacilityAgreed),
					$"{decisionDrawdownFacilityAgreed} value is not one of the supported decision drawdown facility agreed");
			}


			if (decisionFrameworkCategory.HasValue && !Enum.IsDefined(typeof(Enums.Concerns.DecisionFrameworkCategory), decisionFrameworkCategory))
			{
				throw new ArgumentOutOfRangeException(nameof(decisionFrameworkCategory),
					$"{decisionFrameworkCategory} value is not one of the supported decision category");
			}

			DecisionTypeId = decisionType;
			DecisionDrawdownFacilityAgreedId = decisionDrawdownFacilityAgreed;
			DecisionFrameworkCategoryId = decisionFrameworkCategory;
		}

		public int DecisionId { get; set; }
	}
}

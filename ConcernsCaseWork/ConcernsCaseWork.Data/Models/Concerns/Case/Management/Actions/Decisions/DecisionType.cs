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
					"The given value is not one of the supported decision types");
			}
			DecisionTypeId = decisionType;
			DecisionDrawdownFacilityAgreedId = decisionDrawdownFacilityAgreed;
			DecisionFrameworkCategoryId = decisionFrameworkCategory;
		}

		public int DecisionId { get; set; }
	}
}

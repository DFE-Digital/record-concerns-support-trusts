namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions
{
	public class DecisionType
	{
		public Enums.Concerns.DecisionType DecisionTypeId { get; set; }

		private DecisionType()
		{
		}

		public DecisionType(Enums.Concerns.DecisionType decisionType) : this()
		{
			if (!Enum.IsDefined(typeof(Enums.Concerns.DecisionType), decisionType))
			{
				throw new ArgumentOutOfRangeException(nameof(decisionType),
					"The given value is not one of the supported decision types");
			}
			DecisionTypeId = decisionType;
		}

		public int DecisionId { get; set; }
	}
}

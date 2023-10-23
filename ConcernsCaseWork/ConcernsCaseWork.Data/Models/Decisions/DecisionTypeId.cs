namespace ConcernsCaseWork.Data.Models.Decisions
{
	// reference table, not required by app in normal use, but used to give context to the data
	public class DecisionTypeId
	{
		public API.Contracts.Decisions.DecisionType Id { get; private set; }
		public string Name { get; private set; }

		private DecisionTypeId()
		{
		}

		public DecisionTypeId(API.Contracts.Decisions.DecisionType decisionType, string name) : this()
		{
			Id = decisionType;
			Name = name;
		}
	}
}
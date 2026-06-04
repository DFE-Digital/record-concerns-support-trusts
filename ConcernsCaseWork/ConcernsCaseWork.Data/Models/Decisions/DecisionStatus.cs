namespace ConcernsCaseWork.Data.Models.Decisions
{
	public class DecisionStatus
	{
		public API.Contracts.Decisions.DecisionStatus Id { get; set; }
		public string Name { get; set; }

		private DecisionStatus()
		{
		}

		public DecisionStatus(API.Contracts.Decisions.DecisionStatus status) : this()
		{
			if (!Enum.IsDefined(typeof(API.Contracts.Decisions.DecisionStatus), status))
				throw new ArgumentOutOfRangeException(nameof(status),
					"The given value is not one a supported decision status");
			Id = status;
		}
	}
}

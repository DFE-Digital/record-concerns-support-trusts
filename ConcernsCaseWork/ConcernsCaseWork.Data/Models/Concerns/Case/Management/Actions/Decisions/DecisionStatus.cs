namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions
{
	public class DecisionStatus
	{
		public Enums.Concerns.DecisionStatus Id { get; set; }
		public string Name { get; set; }

		private DecisionStatus()
		{
		}

		public DecisionStatus(Enums.Concerns.DecisionStatus status) : this()
		{
			if (!Enum.IsDefined(typeof(Enums.Concerns.DecisionStatus), status))
			{
				throw new ArgumentOutOfRangeException(nameof(status),
					"The given value is not one a supported decision status");
			}
			Id = status;
		}
	}
}

namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions
{
	public class DecisionDrawdownFacilityAgreed
	{
		public Enums.Concerns.DecisionDrawdownFacilityAgreed Id { get; set; }
		public string Name { get; set; }

		private DecisionDrawdownFacilityAgreed()
		{
		}

		public DecisionDrawdownFacilityAgreed(Enums.Concerns.DecisionDrawdownFacilityAgreed facilityAgreed) : this()
		{
			if (!Enum.IsDefined(typeof(Enums.Concerns.DecisionDrawdownFacilityAgreed), facilityAgreed))
			{
				throw new ArgumentOutOfRangeException(nameof(facilityAgreed),
					"The given value is not one a supported decision drawdown facility");
			}
			Id = facilityAgreed;
		}
	}
}

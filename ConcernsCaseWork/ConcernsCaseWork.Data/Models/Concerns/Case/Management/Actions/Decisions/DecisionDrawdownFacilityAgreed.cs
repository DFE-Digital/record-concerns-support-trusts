namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions
{
	public class DecisionDrawdownFacilityAgreed
	{
		public API.Contracts.Decisions.DrawdownFacilityAgreed Id { get; set; }
		public string Name { get; set; }

		private DecisionDrawdownFacilityAgreed()
		{
		}

		public DecisionDrawdownFacilityAgreed(API.Contracts.Decisions.DrawdownFacilityAgreed facilityAgreed) : this()
		{
			if (!Enum.IsDefined(typeof(API.Contracts.Decisions.DrawdownFacilityAgreed), facilityAgreed))
			{
				throw new ArgumentOutOfRangeException(nameof(facilityAgreed),
					"The given value is not one a supported decision drawdown facility");
			}
			Id = facilityAgreed;
		}
	}
}

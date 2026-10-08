namespace ConcernsCaseWork.Data.Models.Decisions
{
	public class DecisionAcademyTrustPayApproval
	{
		public API.Contracts.Decisions.AcademyTrustPayApproval Id { get; set; }
		public string Name { get; set; }

		private DecisionAcademyTrustPayApproval()
		{
		}

		public DecisionAcademyTrustPayApproval(API.Contracts.Decisions.AcademyTrustPayApproval academyTrustPayApproval) : this()
		{
			if (!Enum.IsDefined(typeof(API.Contracts.Decisions.AcademyTrustPayApproval), academyTrustPayApproval))
				throw new ArgumentOutOfRangeException(nameof(academyTrustPayApproval),
					"The given value is not a supported decision academy trust pay approval");
			Id = academyTrustPayApproval;
		}
	}
}

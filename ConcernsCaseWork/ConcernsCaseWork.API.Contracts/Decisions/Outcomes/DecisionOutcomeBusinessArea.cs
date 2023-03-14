using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public enum DecisionOutcomeBusinessArea
	{
		[Description("SFSO (Schools Financial Support and Oversight)")]
		SchoolsFinancialSupportAndOversight = 1,

		[Description("Business Partner")]
		BusinessPartner = 2,

		[Description("Capital")]
		Capital = 3,

		[Description("Funding")]
		Funding = 4,

		[Description("FPMO (Financial Provider Market Oversight)")]
		FinancialProviderMarketOversight = 5,

		[Description("RG (Regions Group)")]
		RegionsGroup = 6
	}
}

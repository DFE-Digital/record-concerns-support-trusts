using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public enum DecisionOutcomeBusinessArea
	{
		[Description("Schools Financial Support and Oversight (SFSO)")]
		SchoolsFinancialSupportAndOversight = 1,

		[Description("Business Partner")]
		BusinessPartner = 2,

		[Description("Capital")]
		Capital = 3,

		[Description("Funding")]
		Funding = 4,

		[Description("Provider Market Oversight (PMO)")]
		ProviderMarketOversight = 5,

		[Description("Regions Group (RG)")]
		RegionsGroup = 6
	}
}

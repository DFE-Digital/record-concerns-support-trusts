using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Enums
{
	public enum BusinessArea
	{
		[Description("Schools Financial Support and Oversight (SFSO)")]
		SchoolsFinancialSupport = 1,

		[Description("Business Partner")]
		BusinessPartner = 2,

		[Description("Capital")]
		Capital = 3,

		[Description("Funding")]
		Funding = 4,

		[Description("Provider Market Oversight (PMO)")]
		ProviderMarketOversight = 5,

		[Description("Regions Group (RG)")]
		RegionsGroup = 6,
	}
}
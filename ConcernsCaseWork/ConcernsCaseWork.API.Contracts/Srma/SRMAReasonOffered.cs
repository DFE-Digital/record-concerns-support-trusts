using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Srma
{
    public enum SRMAReasonOffered
    {
		Unknown = 0,
		[Description("Offer linked with grant funding or other offer of support")]
		OfferLinked = 1,
		[Description("Schools Financial Support and Oversight (SFSO) action")]
		SchoolsFinancialSupportAndOversight = 2,
		[Description("Regions Group (RG) action")]
		RegionsGroupIntervention = 3
	}
}
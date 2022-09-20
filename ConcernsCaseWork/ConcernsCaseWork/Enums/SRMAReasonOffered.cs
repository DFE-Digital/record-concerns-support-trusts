using System.ComponentModel;

namespace ConcernsCaseWork.Enums
{
	public enum SRMAReasonOffered
	{
		Unknown = 0,
		[Description("Offer linked with grant funding or other offer of support")]
		OfferLinked = 1,
		[Description("Schools Financial Support and Oversight ")]
		SchoolsFinancialSupportAndOversight = 2,
		[Description("Regions Group Intervention")]
		RegionsGroupIntervention = 3
	}
}

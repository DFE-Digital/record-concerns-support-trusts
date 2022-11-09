using System.ComponentModel;

namespace ConcernsCaseWork.Service.CaseActions
{
	public enum SRMAReasonOffered
	{
		Unknown = 0,

		[Description("Offer linked with grant funding or other offer of support")]
		OfferLinked = 1,

		[Description("Schools Financial Support and Oversight (SFSO) action")]
		AMSDIntervention = 2,

		[Description("Region Group (RG) Intervention")]
		RDDIntervention = 3
	}
}

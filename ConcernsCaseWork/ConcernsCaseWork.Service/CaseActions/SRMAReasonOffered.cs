using System.ComponentModel;

namespace ConcernsCaseWork.Service.CaseActions
{
	public enum SRMAReasonOffered
	{
		Unknown = 0,

		[Description("Offer linked with grant funding or other offer of support")]
		OfferLinked = 1,

		[Description("Schools Financial Support and Oversight ")]
		AMSDIntervention = 2,

		[Description("Regions Group Intervention")]
		RDDIntervention = 3
	}
}

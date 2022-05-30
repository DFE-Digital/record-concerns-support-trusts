using System.ComponentModel;

namespace ConcernsCaseWork.Enums
{
	public enum SRMAReasonOffered
	{
		Unknown = 0,
		[Description("Offer linked with grant funding or other offer of support")]
		OfferLinked = 1,
		[Description("AMSD Intervention")]
		AMSDIntervention = 2,
		[Description("RDD Intervention")]
		RDDIntervention = 3
	}
}

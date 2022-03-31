using System.ComponentModel;

namespace ConcernsCaseWork.Enums
{
	public enum SRMAReasonOffered
	{
		Unknown,
		[Description("Offer linked with grant funding or other offer of support")]
		OfferLinked,
		[Description("AMSD Intervention")]
		AMSDIntervention,
		[Description("RDD Intervention")]
		RDDIntervention

	}
}

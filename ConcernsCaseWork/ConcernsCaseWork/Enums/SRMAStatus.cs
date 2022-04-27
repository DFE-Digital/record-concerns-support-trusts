using System.ComponentModel;

namespace ConcernsCaseWork.Enums
{
	public enum SRMAStatus
	{
		Unknown,
		
		[Description("Trust considering")]
		TrustConsidering,

		[Description("Preparing for deployment")]
		PreparingForDeployment,

		[Description("Deployed")]
		Deployed,

		[Description("SRMA Declined")]
		Declined,

		[Description("SRMA Canceled")]
		Canceled,

		[Description("SRMA Complete")]
		Complete
	}
}

using System.ComponentModel;

namespace ConcernsCaseWork.Enums
{
	public enum SRMAStatus
	{
		Unknown = 0,
		
		[Description("Trust considering")]
		TrustConsidering = 1,

		[Description("Preparing for deployment")]
		PreparingForDeployment = 2,

		[Description("Deployed")]
		Deployed = 3,

		[Description("SRMA declined")]
		Declined = 4,

		[Description("SRMA cancelled")]
		Cancelled = 5,

		[Description("SRMA complete")]
		Complete = 6
	}
}

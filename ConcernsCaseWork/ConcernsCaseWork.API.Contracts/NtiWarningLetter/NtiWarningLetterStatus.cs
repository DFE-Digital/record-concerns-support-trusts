using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.NtiWarningLetter
{
	public enum NtiWarningLetterStatus
	{
		Unknown = 0,
		[Description("Preparing warning letter")]
		PreparingWarningLetter = 1,
		[Description("Sent to trust")]
		SentToTrust = 2,
		[Description("Cancel warning letter")]
		CancelWarningLetter = 3,
		[Description("Conditions met")]
		ConditionsMet = 4,
		[Description("Escalate to Notice To Improve")]
		EscalateToNoticeToImprove = 5
	}
}

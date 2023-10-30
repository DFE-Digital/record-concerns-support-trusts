using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.NoticeToImprove
{
	public enum NtiStatus
	{
		[Description("Preparing NTI")]
		PreparingNTI = 1,
		[Description("Issued NTI")]
		IssuedNTI = 2,
		[Description("Progress on track")]
		ProgressOnTrack = 3,
		[Description("Evidence of NTI non-compliance")]
		EvidenceOfNTINonCompliance = 4,
		[Description("Serious NTI breaches - considering escalation to TWN")]
		SeriousNTIBreaches = 5,
		[Description("Submission to lift NTI in progress")]
		SubmissionToLiftNTIInProgress = 6,
		[Description("Submission to close NTI in progress")]
		SubmissionToCloseNTIInProgress = 7,
		Lifted = 8,
		Closed = 9,
		Cancelled = 10
	}
}

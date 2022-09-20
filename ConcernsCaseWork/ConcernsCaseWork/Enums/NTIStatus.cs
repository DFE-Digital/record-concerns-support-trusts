using System.ComponentModel;

namespace ConcernsCaseWork.Enums
{
	public enum NTIStatus
	{
		Unknown = 0,
		PreparingNTI = 1,
		IssuedNTI = 2,
		ProgressOnTrack = 3,
		EvidenceOfNTINonCompliance = 4,
		SeriousNTIBreaches = 5,
		SubmissionToLiftNTIInProgress = 6,
		SubmissionToCloseNTIInProgress = 7,
		Lifted = 8,
		Closed = 9,
		Cancelled = 10
	}
}
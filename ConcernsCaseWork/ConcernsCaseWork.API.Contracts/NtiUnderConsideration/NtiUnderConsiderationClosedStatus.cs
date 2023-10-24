using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.NtiUnderConsideration;

public enum NtiUnderConsiderationClosedStatus
{
	[Description("No further action being taken")]
	NoFurtherAction = 1,
	[Description("To be escalated")]
	ToBeEscalated = 2
}
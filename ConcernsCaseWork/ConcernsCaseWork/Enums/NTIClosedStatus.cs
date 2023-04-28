using System.ComponentModel;

namespace ConcernsCaseWork.Enums;

public enum NtiClosedStatus
{
	[Description("No further action being taken")]
	NoFurtherAction =1,
	[Description("To be escalated")]
	ToBeEscalated =2
}
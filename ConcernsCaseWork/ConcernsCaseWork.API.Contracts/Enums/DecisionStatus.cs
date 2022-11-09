using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Enums
{
    public enum DecisionStatus
    {
	    [Description("In progress")]
	    InProgress = 1,

	    [Description("Closed")]
	    Closed = 2,
    }
}

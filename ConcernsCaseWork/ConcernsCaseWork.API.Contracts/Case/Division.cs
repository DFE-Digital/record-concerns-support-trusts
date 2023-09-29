using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Case;

public enum Division
{
	[Description("SFSO (Schools Financial Support and Oversight)")]
	SFSO = 1,
	[Description("Regions Group")]
	RegionsGroup = 2,
}
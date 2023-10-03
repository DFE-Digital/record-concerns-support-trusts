using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Case
{
	public enum Region
	{
		[Description("North England")]
		NorthEngland = 1,
		[Description("South England")]
		SouthEngland = 2,
		[Description("East England")]
		EastEngland = 3,
		[Description("West England")]
		WestEngland = 4,
	}
}

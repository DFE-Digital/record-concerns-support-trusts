using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Concerns
{
	public enum ConcernRating
	{
		[Description("Red-Plus")]
		RedPlus = 1,

		[Description("Red")]
		Red = 2,

		[Description("Red-Amber")]
		RedAmber = 3,

		[Description("Amber-Green")]
		AmberGreen = 4,

		[Description("n/a")]
		NotApplicable = 5
	}
}

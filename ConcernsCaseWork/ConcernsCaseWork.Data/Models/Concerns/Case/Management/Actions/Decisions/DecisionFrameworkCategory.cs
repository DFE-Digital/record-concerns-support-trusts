namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions
{
	public class DecisionFrameworkCategory
	{
		public API.Contracts.Decisions.FrameworkCategory Id { get; set; }
		public string Name { get; set; }

		private DecisionFrameworkCategory()
		{
		}

		public DecisionFrameworkCategory(API.Contracts.Decisions.FrameworkCategory frameworkCategory) : this()
		{
			if (!Enum.IsDefined(typeof(API.Contracts.Decisions.FrameworkCategory), frameworkCategory))
			{
				throw new ArgumentOutOfRangeException(nameof(frameworkCategory),
					"The given value is not a supported decision framework category");
			}
			Id = frameworkCategory;
		}
	}
}

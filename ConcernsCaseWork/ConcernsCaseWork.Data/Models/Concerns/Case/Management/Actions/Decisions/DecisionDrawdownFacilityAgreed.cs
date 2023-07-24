namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions
{
	public class DecisionFrameworkCategory
	{
		public Enums.Concerns.DecisionFrameworkCategory Id { get; set; }
		public string Name { get; set; }

		private DecisionFrameworkCategory()
		{
		}

		public DecisionFrameworkCategory(Enums.Concerns.DecisionFrameworkCategory frameworkCategory) : this()
		{
			if (!Enum.IsDefined(typeof(Enums.Concerns.DecisionFrameworkCategory), frameworkCategory))
			{
				throw new ArgumentOutOfRangeException(nameof(frameworkCategory),
					"The given value is not a supported decision framework category");
			}
			Id = frameworkCategory;
		}
	}
}

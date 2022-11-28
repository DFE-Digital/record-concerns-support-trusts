namespace ConcernsCaseWork.Models.CaseActions
{
	public sealed class FinancialPlanStatusModel
	{
		public string Name { get; }
		public long? Id { get; set; }
		public bool IsClosedStatus { get; }

		public FinancialPlanStatusModel()
		{ 

		}

		public FinancialPlanStatusModel(string name, long id, bool isClosedStatus) =>
			(Name, Id, IsClosedStatus) = (name, id, isClosedStatus);
	}
}

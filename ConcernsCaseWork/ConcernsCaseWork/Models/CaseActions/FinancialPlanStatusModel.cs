namespace ConcernsCaseWork.Models.CaseActions
{
	public sealed class FinancialPlanStatusModel
	{
		public string Name { get; }
		public long Id { get; }
		public bool IsClosedStatus { get; }

		public FinancialPlanStatusModel(string name, long id, bool isClosedStatus) =>
			(Name, Id, IsClosedStatus) = (name, id, isClosedStatus);
	}
}

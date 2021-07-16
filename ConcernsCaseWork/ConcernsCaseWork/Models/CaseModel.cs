namespace ConcernsCaseWork.Models
{
	public sealed class CaseModel
	{
		public string Id { get; }
		public string Type { get; }
		public string TrustName { get; }
		public int Rag { get; }
		public int DaysOpen { get; }

		public CaseModel(string id, string type, string trustName, int rag, int daysOpen)
		{
			Id = id;
			Type = type;
			TrustName = trustName;
			Rag = rag;
			DaysOpen = daysOpen;
		}
	}
}
namespace ConcernsCaseWork.Models
{
	public sealed class CaseModel
	{
		public string Id { get; }
		public string Type { get; }
		public string TrustName { get; }
		public int Rag { get; }
		public string Created { get; }
		public string LastUpdate { get; }
		public string Closed { get; }

		public CaseModel(string id, string type, string trustName, int rag, string created, string lastUpdate, string closed)
		{
			Id = id;
			Type = type;
			TrustName = trustName;
			Rag = rag;
			Created = created;
			LastUpdate = lastUpdate;
			Closed = closed;
		}
	}
}
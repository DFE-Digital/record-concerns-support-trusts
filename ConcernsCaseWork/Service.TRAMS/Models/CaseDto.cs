using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class CaseDto
	{
		public string Id { get; }
		public string Type { get; }
		public string TrustName { get; }
		public int Rag { get; }
		public int DaysOpen { get; }

		[JsonConstructor]
		public CaseDto(string id, string type, string trustName, int rag, int daysOpen)
		{
			Id = id;
			Type = type;
			TrustName = trustName;
			Rag = rag;
			DaysOpen = daysOpen;
		}
	}
}
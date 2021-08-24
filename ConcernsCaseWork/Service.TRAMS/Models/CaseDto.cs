using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class CaseDto
	{
		[JsonPropertyName("id")]
		public string Id { get; }
		
		[JsonPropertyName("type")]
		public string Type { get; }
		
		[JsonPropertyName("trustName")]
		public string TrustName { get; }
		
		[JsonPropertyName("rag")]
		public int Rag { get; }
		
		[JsonPropertyName("daysOpen")]
		public int DaysOpen { get; }

		[JsonConstructor]
		public CaseDto(string id, string type, string trustName, int rag, int daysOpen) => 
			(Id, Type, TrustName, Rag, DaysOpen) = (id, type, trustName, rag, daysOpen);
	}
}
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
		
		[JsonPropertyName("created")]
		public string Created { get; }

		[JsonPropertyName("lastUpdate")]
		public string LastUpdate { get; }
		
		[JsonPropertyName("closed")]
		public string Closed { get; }
		
		[JsonConstructor]
		public CaseDto(string id, string type, string trustName, int rag, string created, string lastUpdate, string closed) => 
			(Id, Type, TrustName, Rag, Created, LastUpdate, Closed) = (id, type, trustName, rag, created, lastUpdate, closed);
	}
}
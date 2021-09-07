using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class RecordWhistleblowerDto
	{
		[JsonPropertyName("id")]
		public int Id { get; }
		
		[JsonPropertyName("name")]
		public string Name { get; }
		
		[JsonPropertyName("details")]
		public string Details { get; }
		
		[JsonPropertyName("reason")]
		public string Reason { get; }
		
		[JsonPropertyName("record_id")]
		public int RecordId { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		public RecordWhistleblowerDto(int id, string name, string details, string reason, int recordId, BigInteger urn) => 
			(Id, Name, Details, Reason, RecordId, Urn) = (id, name, details, reason, recordId, urn);
	}
}
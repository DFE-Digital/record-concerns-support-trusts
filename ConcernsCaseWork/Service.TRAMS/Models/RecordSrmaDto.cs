using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class RecordSrmaDto
	{
		[JsonPropertyName("name")]
		public string Name { get; }
		
		[JsonPropertyName("details")]
		public string Details { get; }
		
		[JsonPropertyName("reason")]
		public string Reason { get; }
		
		[JsonPropertyName("record_urn")]
		public BigInteger RecordUrn { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		public RecordSrmaDto(int id, string name, string details, string reason, BigInteger recordUrn, BigInteger urn) => 
			(Name, Details, Reason, RecordUrn, Urn) = (name, details, reason, recordUrn, urn);
	}
}
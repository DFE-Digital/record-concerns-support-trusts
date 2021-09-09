using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Cases
{
	public sealed class RecordWhistleblowerDto
	{
		[JsonPropertyName("name")]
		public string Name { get; }
		
		[JsonPropertyName("details")]
		public string Details { get; }
		
		[JsonPropertyName("reason")]
		public string Reason { get; }
		
		[JsonPropertyName("record_urn")]
		public int RecordUrn { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		[JsonConstructor]
		public RecordWhistleblowerDto(string name, string details, string reason, int recordUrn, BigInteger urn) => 
			(Name, Details, Reason, RecordUrn, Urn) = (name, details, reason, recordUrn, urn);
	}
}
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.RecordAcademy
{
	public sealed class CreateRecordWhistleblowerDto
	{
		[JsonPropertyName("name")]
		public string Name { get; }
		
		[JsonPropertyName("details")]
		public string Details { get; }
		
		[JsonPropertyName("reason")]
		public string Reason { get; }
		
		[JsonPropertyName("record_urn")]
		public BigInteger RecordUrn { get; }
		
		[JsonConstructor]
		public CreateRecordWhistleblowerDto(string name, string details, string reason, BigInteger recordUrn) => 
			(Name, Details, Reason, RecordUrn) = (name, details, reason, recordUrn);
	}
}
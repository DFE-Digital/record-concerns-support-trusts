using System.Text.Json.Serialization;

namespace Service.TRAMS.RecordSrma
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
		public long RecordUrn { get; }
		
		[JsonPropertyName("urn")]
		public long Urn { get; }
		
		[JsonConstructor]
		public RecordSrmaDto(int id, string name, string details, string reason, long recordUrn, long urn) => 
			(Name, Details, Reason, RecordUrn, Urn) = (name, details, reason, recordUrn, urn);
	}
}
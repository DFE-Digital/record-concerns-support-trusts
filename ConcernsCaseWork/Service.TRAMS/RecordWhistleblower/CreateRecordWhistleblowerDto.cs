using Newtonsoft.Json;

namespace Service.TRAMS.RecordWhistleblower
{
	public sealed class CreateRecordWhistleblowerDto
	{
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("details")]
		public string Details { get; }
		
		[JsonProperty("reason")]
		public string Reason { get; }
		
		[JsonProperty("record_urn")]
		public long RecordUrn { get; }
		
		[JsonConstructor]
		public CreateRecordWhistleblowerDto(string name, string details, string reason, long recordUrn) => 
			(Name, Details, Reason, RecordUrn) = (name, details, reason, recordUrn);
	}
}
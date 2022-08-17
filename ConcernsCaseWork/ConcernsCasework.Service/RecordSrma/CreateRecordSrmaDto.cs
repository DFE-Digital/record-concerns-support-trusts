using Newtonsoft.Json;

namespace ConcernsCasework.Service.RecordSrma
{
	public sealed class CreateRecordSrmaDto
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
		public CreateRecordSrmaDto(string name, string details, string reason, long recordUrn) => 
			(Name, Details, Reason, RecordUrn) = (name, details, reason, recordUrn);
	}
}
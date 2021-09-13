using Newtonsoft.Json;
using System.Numerics;

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
		public BigInteger RecordUrn { get; }
		
		[JsonConstructor]
		public CreateRecordWhistleblowerDto(string name, string details, string reason, BigInteger recordUrn) => 
			(Name, Details, Reason, RecordUrn) = (name, details, reason, recordUrn);
	}
}
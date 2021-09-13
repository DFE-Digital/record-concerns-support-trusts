using Newtonsoft.Json;
using System.Numerics;

namespace Service.TRAMS.RecordSrma
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
		public BigInteger RecordUrn { get; }
		
		[JsonConstructor]
		public CreateRecordSrmaDto(int id, string name, string details, string reason, BigInteger recordUrn) => 
			(Name, Details, Reason, RecordUrn) = (name, details, reason, recordUrn);
	}
}
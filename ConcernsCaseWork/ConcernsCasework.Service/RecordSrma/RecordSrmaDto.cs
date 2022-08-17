﻿using Newtonsoft.Json;

namespace ConcernsCasework.Service.RecordSrma
{
	public sealed class RecordSrmaDto
	{
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("details")]
		public string Details { get; }
		
		[JsonProperty("reason")]
		public string Reason { get; }
		
		[JsonProperty("record_urn")]
		public long RecordUrn { get; }
		
		[JsonProperty("urn")]
		public long Urn { get; }
		
		[JsonConstructor]
		public RecordSrmaDto(string name, string details, string reason, long recordUrn, long urn) => 
			(Name, Details, Reason, RecordUrn, Urn) = (name, details, reason, recordUrn, urn);
	}
}
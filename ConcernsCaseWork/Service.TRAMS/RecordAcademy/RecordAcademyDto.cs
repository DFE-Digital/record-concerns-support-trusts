using Newtonsoft.Json;

namespace Service.TRAMS.RecordAcademy
{
	public sealed class RecordAcademyDto
	{
		[JsonProperty("record_urn")]
		public long RecordUrn { get; }
		
		[JsonProperty("academy_urn")]
		public int AcademyUrn { get; }
		
		[JsonProperty("urn")]
		public long Urn { get; }
		
		[JsonConstructor]
		public RecordAcademyDto(long recordUrn, int academyUrn, long urn) => 
			(RecordUrn, AcademyUrn, Urn) = (recordUrn, academyUrn, urn);
	}
}
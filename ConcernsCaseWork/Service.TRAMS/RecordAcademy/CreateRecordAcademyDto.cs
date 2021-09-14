using Newtonsoft.Json;

namespace Service.TRAMS.RecordAcademy
{
	public sealed class CreateRecordAcademyDto
	{
		[JsonProperty("record_urn")]
		public long RecordUrn { get; }
		
		[JsonProperty("academy_urn")]
		public int AcademyUrn { get; }
		
		[JsonConstructor]
		public CreateRecordAcademyDto(long recordUrn, int academyUrn) => 
			(RecordUrn, AcademyUrn) = (recordUrn, academyUrn);
	}
}
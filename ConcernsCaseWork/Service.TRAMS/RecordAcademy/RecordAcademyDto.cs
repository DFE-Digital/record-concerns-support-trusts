using System.Text.Json.Serialization;

namespace Service.TRAMS.RecordAcademy
{
	public sealed class RecordAcademyDto
	{
		[JsonPropertyName("record_urn")]
		public long RecordUrn { get; }
		
		[JsonPropertyName("academy_urn")]
		public int AcademyUrn { get; }
		
		[JsonPropertyName("urn")]
		public long Urn { get; }
		
		[JsonConstructor]
		public RecordAcademyDto(long recordUrn, int academyUrn, long urn) => 
			(RecordUrn, AcademyUrn, Urn) = (recordUrn, academyUrn, urn);
	}
}
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.RecordAcademy
{
	public sealed class CreateRecordAcademyDto
	{
		[JsonPropertyName("record_urn")]
		public BigInteger RecordUrn { get; }
		
		[JsonPropertyName("academy_urn")]
		public int AcademyUrn { get; }
		
		[JsonConstructor]
		public CreateRecordAcademyDto(BigInteger recordUrn, int academyUrn) => 
			(RecordUrn, AcademyUrn) = (recordUrn, academyUrn);
	}
}
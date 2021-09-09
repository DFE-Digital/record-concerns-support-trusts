using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Cases
{
	public sealed class RecordAcademyDto
	{
		[JsonPropertyName("record_urn")]
		public BigInteger RecordUrn { get; }
		
		[JsonPropertyName("academy_urn")]
		public int AcademyUrn { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		[JsonConstructor]
		public RecordAcademyDto(BigInteger recordUrn, int academyUrn, BigInteger urn) => 
			(RecordUrn, AcademyUrn, Urn) = (recordUrn, academyUrn, urn);
	}
}
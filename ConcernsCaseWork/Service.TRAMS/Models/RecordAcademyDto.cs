using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class RecordAcademyDto
	{
		[JsonPropertyName("id")]
		public int Id { get; }
		
		[JsonPropertyName("record_id")]
		public int RecordId { get; }
		
		[JsonPropertyName("academy_urn")]
		public int AcademyUrn { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		public RecordAcademyDto(int id, int recordId, int academyUrn, BigInteger urn) => 
			(Id, RecordId, AcademyUrn, Urn) = (id, recordId, academyUrn, urn);
	}
}
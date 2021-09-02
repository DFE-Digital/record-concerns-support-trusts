using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class TrustDetailsDto
	{
		[JsonPropertyName("giasData")]
		public GiasDataDto GiasData { get; }

		[JsonConstructor]
		public TrustDetailsDto(GiasDataDto giasData) => (GiasData) = (giasData);
	}
}
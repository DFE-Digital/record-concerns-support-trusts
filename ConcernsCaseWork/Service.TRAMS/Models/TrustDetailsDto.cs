using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class TrustDetailsDto
	{
		[JsonPropertyName("giasData")]
		public GiasDataDto GiasData { get; }
		
		[JsonPropertyName("establishments")]
		public List<EstablishmentDto> Establishments { get; } 

		[JsonConstructor]
		public TrustDetailsDto(GiasDataDto giasData, List<EstablishmentDto> establishments) => 
			(GiasData, Establishments) = (giasData, establishments);
	}
}
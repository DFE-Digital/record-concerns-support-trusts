using Newtonsoft.Json;
using System.Collections.Generic;

namespace Service.TRAMS.Trusts
{
	public sealed class TrustDetailsDto
	{
		[JsonProperty("giasData")]
		public GiasDataDto GiasData { get; }
		
		[JsonProperty("establishments")]
		public List<EstablishmentDto> Establishments { get; } 

		[JsonConstructor]
		public TrustDetailsDto(GiasDataDto giasData, List<EstablishmentDto> establishments) => 
			(GiasData, Establishments) = (giasData, establishments);
	}
}
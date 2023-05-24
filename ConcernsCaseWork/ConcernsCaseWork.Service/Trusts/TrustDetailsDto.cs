using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class TrustDetailsDto
	{
		[JsonProperty("giasData")]
		public virtual GiasDataDto GiasData { get; set; }
		
		[JsonProperty("ifdData")]
		public virtual IfdDataDto IfdData { get; set; }
		
		[JsonProperty("establishments")]
		public virtual List<EstablishmentDto> Establishments { get; set; } 

		[JsonConstructor]
		public TrustDetailsDto(GiasDataDto giasData, IfdDataDto ifdData, List<EstablishmentDto> establishments) => 
			(GiasData, IfdData, Establishments) = (giasData, ifdData, establishments);

		public TrustDetailsDto() {}
	}
}
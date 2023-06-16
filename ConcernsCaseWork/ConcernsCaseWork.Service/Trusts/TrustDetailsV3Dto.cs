using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class TrustDetailsV3Dto
	{
		[JsonProperty("giasData")]
		public virtual GiasDataDto GiasData { get; set; }
		
		[JsonProperty("trustData")]
		public virtual IfdDataDto TrustData { get; set; }
		
		[JsonProperty("establishments")]
		public virtual List<EstablishmentDto> Establishments { get; set; } 

		[JsonConstructor]
		public TrustDetailsV3Dto(GiasDataDto giasData, IfdDataDto trustData, List<EstablishmentDto> establishments) => 
			(GiasData, TrustData, Establishments) = (giasData, trustData, establishments);

		public TrustDetailsV3Dto() {}
	}
}
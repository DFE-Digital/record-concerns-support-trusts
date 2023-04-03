using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class EstablishmentSummaryDto
	{
		[JsonProperty("urn")]
		public virtual string Urn { get; set; }
		
		[JsonProperty("name")]
		public virtual string Name { get; set; }
		
		[JsonProperty("ukprn")]
		public virtual string UkPrn { get; set; }
		
		[JsonConstructor]
		public EstablishmentSummaryDto(string urn, string name, string ukprn) => 
			(Urn, Name, UkPrn) = (urn, name, ukprn);
		
		protected EstablishmentSummaryDto() { }
	}
}
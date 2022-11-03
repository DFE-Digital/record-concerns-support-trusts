using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public sealed class EstablishmentSummaryDto
	{
		[JsonProperty("urn")]
		public string Urn { get; }
		
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("ukprn")]
		public string UkPrn { get; }
		
		[JsonConstructor]
		public EstablishmentSummaryDto(string urn, string name, string ukprn) => 
			(Urn, Name, UkPrn) = (urn, name, ukprn);
	}
}
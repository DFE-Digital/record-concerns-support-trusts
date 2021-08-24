using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class EstablishmentDto
	{
		public string Urn { get; }
		public string Name { get; }
		public string UkPrn { get; }
		
		[JsonConstructor]
		public EstablishmentDto(string urn, string name, string ukprn)
		{
			Urn = urn;
			Name = name;
			UkPrn = ukprn;
		}
	}
}
using Newtonsoft.Json;

namespace Service.TRAMS.Models
{
	/// <summary>
	/// TODO missing mappings from the real response.
	/// </summary>
	public sealed class EstablishmentDto
	{
		public AddressDto Address { get; }
		public NameAndCodeDto AdministrativeWard { get; }
		public NameAndCodeDto AdmissionsPolicy { get; }
		public NameAndCodeDto Boarders { get; }
		public string BoardingEstablishment { get; }
		public string Ccf { get; }

		[JsonConstructor]
		public EstablishmentDto(AddressDto address, NameAndCodeDto administrativeWard, NameAndCodeDto admissionsPolicy, NameAndCodeDto boarders, string boardingEstablishment, string ccf)
		{
			Address = address;
			AdministrativeWard = administrativeWard;
			AdmissionsPolicy = admissionsPolicy;
			Boarders = boarders;
			BoardingEstablishment = boardingEstablishment;
			Ccf = ccf;
		}
	}
}
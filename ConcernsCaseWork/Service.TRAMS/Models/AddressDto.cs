namespace Service.TRAMS.Models
{
	public sealed class AddressDto
	{
		public string AdditionalLine { get; }
		public string County { get; }
		public string Locality { get; }
		public string Postcode { get; }
		public string Street { get; }
		public string Town { get; }

		public AddressDto(string additionalLine, string county, string locality, string postcode, string street, string town)
		{
			AdditionalLine = additionalLine;
			County = county;
			Locality = locality;
			Postcode = postcode;
			Street = street;
			Town = town;
		}
	}
}
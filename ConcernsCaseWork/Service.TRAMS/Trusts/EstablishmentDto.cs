using Newtonsoft.Json;

namespace Service.TRAMS.Trusts
{
	public sealed class EstablishmentDto
	{
		[JsonProperty("urn")]
		public string Urn { get; }
		
		[JsonProperty("localAuthorityCode")]
		public string LocalAuthorityCode { get; }
		
		[JsonProperty("localAuthorityName")]
		public string LocalAuthorityName { get; }
		
		[JsonProperty("establishmentNumber")]
		public string EstablishmentNumber { get; }
		
		[JsonProperty("establishmentName")]
		public string EstablishmentName { get; }

		[JsonProperty("headteacherTitle")]
		public string HeadteacherTitle { get; }

		[JsonProperty("headteacherFirstName")]
		public string HeadteacherFirstName { get; }

		[JsonProperty("headteacherLastName")]
		public string HeadteacherLastName { get; }

		[JsonProperty("establishmentType")]
		public EstablishmentTypeDto EstablishmentType { get; }


		[JsonConstructor]
		public EstablishmentDto(string urn, string localAuthorityCode, string localAuthorityName,
			string establishmentNumber, string establishmentName, string headteacherTitle, string headteacherFirstName, string headteacherLastName, EstablishmentTypeDto establishmentType) => 
			(Urn, LocalAuthorityCode, LocalAuthorityName, EstablishmentNumber, EstablishmentName, HeadteacherTitle, HeadteacherFirstName, HeadteacherLastName, EstablishmentType) = 
			(urn, localAuthorityCode, localAuthorityName, establishmentNumber, establishmentName, headteacherTitle, headteacherFirstName, headteacherLastName, establishmentType);
	}
}
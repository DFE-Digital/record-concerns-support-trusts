using Newtonsoft.Json;

namespace ConcernsCasework.Service.Trusts
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

		[JsonProperty("census")]
		public CensusDto Census { get; }

		[JsonProperty("schoolWebsite")]
		public string SchoolWebsite { get; }

		[JsonProperty("schoolCapacity")]
		public string SchoolCapacity { get; }		

		[JsonConstructor]
		public EstablishmentDto(string urn, string localAuthorityCode, string localAuthorityName,
			string establishmentNumber, string establishmentName, string headteacherTitle, string headteacherFirstName, string headteacherLastName, EstablishmentTypeDto establishmentType, CensusDto census, string schoolWebsite, string schoolCapacity) => 
			(Urn, LocalAuthorityCode, LocalAuthorityName, EstablishmentNumber, EstablishmentName, HeadteacherTitle, HeadteacherFirstName, HeadteacherLastName, EstablishmentType, Census, SchoolWebsite, SchoolCapacity) = 
			(urn, localAuthorityCode, localAuthorityName, establishmentNumber, establishmentName, headteacherTitle, headteacherFirstName, headteacherLastName, establishmentType, census, schoolWebsite, schoolCapacity);
	}
}
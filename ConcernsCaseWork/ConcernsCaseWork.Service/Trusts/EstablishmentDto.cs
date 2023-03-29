using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class EstablishmentDto
	{
		[JsonProperty("urn")]
		public virtual string Urn { get; set; }
		
		[JsonProperty("localAuthorityCode")]
		public virtual string LocalAuthorityCode { get; set; }
		
		[JsonProperty("localAuthorityName")]
		public virtual string LocalAuthorityName { get; set; }
		
		[JsonProperty("establishmentNumber")]
		public virtual string EstablishmentNumber { get; set; }
		
		[JsonProperty("establishmentName")]
		public virtual string EstablishmentName { get; set; }

		[JsonProperty("headteacherTitle")]
		public virtual string HeadteacherTitle { get; set; }

		[JsonProperty("headteacherFirstName")]
		public virtual string HeadteacherFirstName { get; set; }

		[JsonProperty("headteacherLastName")]
		public virtual string HeadteacherLastName { get; set; }

		[JsonProperty("establishmentType")]
		public virtual EstablishmentTypeDto EstablishmentType { get; set; }

		[JsonProperty("census")]
		public virtual CensusDto Census { get; set; }

		[JsonProperty("schoolWebsite")]
		public virtual string SchoolWebsite { get; set; }

		[JsonProperty("schoolCapacity")]
		public virtual string SchoolCapacity { get; set; }		

		[JsonConstructor]
		public EstablishmentDto(string urn, string localAuthorityCode, string localAuthorityName,
			string establishmentNumber, string establishmentName, string headteacherTitle, string headteacherFirstName, string headteacherLastName, EstablishmentTypeDto establishmentType, CensusDto census, string schoolWebsite, string schoolCapacity) => 
			(Urn, LocalAuthorityCode, LocalAuthorityName, EstablishmentNumber, EstablishmentName, HeadteacherTitle, HeadteacherFirstName, HeadteacherLastName, EstablishmentType, Census, SchoolWebsite, SchoolCapacity) = 
			(urn, localAuthorityCode, localAuthorityName, establishmentNumber, establishmentName, headteacherTitle, headteacherFirstName, headteacherLastName, establishmentType, census, schoolWebsite, schoolCapacity);
		
		protected EstablishmentDto() { }

	}
}
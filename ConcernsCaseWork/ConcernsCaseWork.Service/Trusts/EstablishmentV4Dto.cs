using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class EstablishmentV4Dto
	{
		[JsonProperty("urn")]
		public virtual string Urn { get; set; }

		[JsonProperty("localAuthorityCode")]
		public virtual string LocalAuthorityCode { get; set; }

		[JsonProperty("localAuthorityName")]
		public virtual string LocalAuthorityName { get; set; }

		[JsonProperty("establishmentNumber")]
		public virtual string EstablishmentNumber { get; set; }

		[JsonProperty("name")]
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

		[JsonProperty("misEstablishments")]
		public MisEstablishmentsV4 MisEstablishments { get; set; } = new();

		protected EstablishmentV4Dto() { }
	}

	public class  MisEstablishmentsV4
	{
		[JsonProperty("webLink")]
		public virtual string SchoolWebsite { get; set; }
	}
}

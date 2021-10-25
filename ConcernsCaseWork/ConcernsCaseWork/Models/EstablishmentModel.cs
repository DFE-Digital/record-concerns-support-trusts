using ConcernsCaseWork.Extensions;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class EstablishmentModel
	{
		public string Urn { get; }
		
		public string LocalAuthorityCode { get; }
		
		public string LocalAuthorityName { get; }
		
		public string EstablishmentNumber { get; }
		
		public string EstablishmentName { get; }

		public string HeadteacherTitle { get; }

		public string HeadteacherFirstName { get; }

		public string HeadteacherLastName { get; }

		public string HeadteacherFullName { get { return $"{HeadteacherTitle} {HeadteacherFirstName} {HeadteacherLastName}"; } }

		public EstablishmentTypeModel EstablishmentType { get; } 

		public CensusModel Census { get; } 

		private string SchoolWebsite { get; }

		public string EstablishmentWebsite { get { return SchoolWebsite.ToUrl(); } }

		public string SchoolCapacity { get; }

		public EstablishmentModel(string urn, string localAuthorityCode, string localAuthorityName,
			string establishmentNumber, string establishmentName, string headteacherTitle, string headteacherFirstName,  string headteacherLastName, EstablishmentTypeModel establishmentType, CensusModel census, string schoolWebsite, string schoolCapacity) => 
			(Urn, LocalAuthorityCode, LocalAuthorityName, EstablishmentNumber, EstablishmentName, HeadteacherTitle, HeadteacherFirstName, HeadteacherLastName, EstablishmentType, Census, SchoolWebsite, SchoolCapacity) = 
			(urn, localAuthorityCode, localAuthorityName, establishmentNumber, establishmentName, headteacherTitle, headteacherFirstName, headteacherLastName, establishmentType, census, schoolWebsite, schoolCapacity);
	}
}